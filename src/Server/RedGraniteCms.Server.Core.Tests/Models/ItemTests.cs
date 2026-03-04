using RedGraniteCms.Server.Core.Models;
using System.Text.Json.Nodes;

namespace RedGraniteCms.Server.Core.Tests.Models;

public class ItemTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsItem()
    {
        // Arrange & Act
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Test Item",
            summary: "Short summary",
            content: "Full content body"
        );

        // Assert
        item.Should().NotBeNull();
        item.Title.Should().Be("Test Item");
        item.Summary.Should().Be("Short summary");
        item.Content.Should().Be("Full content body");
        item.OwnerId.Should().Be("user-1");
        item.ContentType.Should().Be("blog-post");
        item.Id.Should().NotBe(Guid.Empty);
        item.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        item.LastModifiedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        item.Status.Should().Be(ItemStatus.Draft);
        item.Visibility.Should().Be(ItemVisibility.Private);
        item.Language.Should().Be("en");
    }

    [Fact]
    public void Create_WithAllOptions_SetsAllProperties()
    {
        // Arrange
        var metadata = new JsonObject { ["hero"] = "https://example.com/hero.jpg" };
        var tags = new[] { "tech", "news" };

        // Act
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Full Item",
            summary: "Summary",
            content: "Content",
            status: ItemStatus.Published,
            visibility: ItemVisibility.Public,
            language: "fr",
            slug: "full-item",
            parentId: "parent-1",
            ancestorIds: new[] { "root", "parent-1" },
            sortOrder: 5,
            metadata: metadata,
            tags: tags,
            createdBy: "admin"
        );

        // Assert
        item.Status.Should().Be(ItemStatus.Published);
        item.Visibility.Should().Be(ItemVisibility.Public);
        item.Language.Should().Be("fr");
        item.Slug.Should().Be("full-item");
        item.ParentId.Should().Be("parent-1");
        item.AncestorIds.Should().BeEquivalentTo(new[] { "root", "parent-1" });
        item.SortOrder.Should().Be(5);
        item.Tags.Should().Contain("tech").And.Contain("news");
        item.CreatedBy.Should().Be("admin");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidOwnerId_ThrowsArgumentException(string? invalidOwnerId)
    {
        var act = () => Item.Create(
            ownerId: invalidOwnerId!,
            contentType: "blog-post",
            title: "Test");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("ownerId");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidContentType_ThrowsArgumentException(string? invalidContentType)
    {
        var act = () => Item.Create(
            ownerId: "user-1",
            contentType: invalidContentType!,
            title: "Test");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("contentType");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        var act = () => Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: invalidTitle!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Create_WithInvalidSlug_ThrowsArgumentException()
    {
        var act = () => Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Test",
            slug: "Invalid Slug!");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("slug");
    }

    [Fact]
    public void Update_WithValidInputs_UpdatesItem()
    {
        // Arrange
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Original Title");
        var originalLastModified = item.LastModifiedAt;

        // Wait briefly to ensure LastModifiedAt changes
        Thread.Sleep(10);

        // Act
        item.Update(
            title: "Updated Title",
            summary: "Updated Summary",
            content: "Updated Content");

        // Assert
        item.Title.Should().Be("Updated Title");
        item.Summary.Should().Be("Updated Summary");
        item.Content.Should().Be("Updated Content");
        item.LastModifiedAt.Should().BeAfter(originalLastModified);
    }

    [Fact]
    public void Update_WithInvalidTitle_ThrowsArgumentException()
    {
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Original Title");

        var act = () => item.Update(title: "");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("title");
    }

    [Fact]
    public void Publish_SetsStatusAndPublishedAt()
    {
        // Arrange
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Draft Post");

        // Act
        item.Publish("editor");

        // Assert
        item.Status.Should().Be(ItemStatus.Published);
        item.PublishedAt.Should().NotBeNull();
        item.PublishedBy.Should().Be("editor");
    }

    [Fact]
    public void Archive_SetsStatusToArchived()
    {
        // Arrange
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Post",
            status: ItemStatus.Published);

        // Act
        item.Archive("admin");

        // Assert
        item.Status.Should().Be(ItemStatus.Archived);
    }

    [Fact]
    public void AddTag_AddsNewTag()
    {
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Post");

        item.AddTag("tech");

        item.Tags.Should().Contain("tech");
    }

    [Fact]
    public void RemoveTag_RemovesExistingTag()
    {
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Post",
            tags: new[] { "tech", "news" });

        item.RemoveTag("tech");

        item.Tags.Should().NotContain("tech");
        item.Tags.Should().Contain("news");
    }

    [Fact]
    public void SetMetadata_SetsAndGetsMetadataValues()
    {
        var item = Item.Create(
            ownerId: "user-1",
            contentType: "blog-post",
            title: "Post");

        item.SetMetadata("hero", JsonValue.Create("https://example.com/hero.jpg"));

        item.GetMetadataString("hero").Should().Be("https://example.com/hero.jpg");
    }
}
