using Microsoft.Extensions.Logging;
using RedGraniteCms.Server.Core.Exceptions;
using RedGraniteCms.Server.Core.Interfaces;
using RedGraniteCms.Server.Core.Models;
using RedGraniteCms.Server.Services;

namespace RedGraniteCms.Server.Services.Tests;

public class ItemServiceTests
{
    private readonly Mock<IItemRepository> _mockRepository;
    private readonly Mock<ILogger<ItemService>> _mockLogger;
    private readonly ItemService _service;

    public ItemServiceTests()
    {
        _mockRepository = new Mock<IItemRepository>();
        _mockLogger = new Mock<ILogger<ItemService>>();
        _service = new ItemService(_mockRepository.Object, _mockLogger.Object);
    }

    private static Item CreateTestItem(string title = "Test", string ownerId = "user-1", string contentType = "blog-post")
        => Item.Create(ownerId: ownerId, contentType: contentType, title: title);

    [Fact]
    public async Task GetItemAsync_ById_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var expectedItem = CreateTestItem();
        _mockRepository.Setup(r => r.GetItemAsync(itemId, null, null, null))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _service.GetItemAsync(id: itemId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedItem);
        _mockRepository.Verify(r => r.GetItemAsync(itemId, null, null, null), Times.Once);
    }

    [Fact]
    public async Task GetItemAsync_ById_WhenItemNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetItemAsync(itemId, null, null, null))
            .ReturnsAsync((Item?)null);

        // Act
        var act = async () => await _service.GetItemAsync(id: itemId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.EntityType == nameof(Item) && e.EntityId == itemId.ToString());
    }

    [Fact]
    public async Task GetItemAsync_BySlugWithFilters_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var expectedItem = CreateTestItem("Slug Item");
        _mockRepository.Setup(r => r.GetItemAsync(null, "test-slug", ItemStatus.Published, ItemVisibility.Public))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _service.GetItemAsync(slug: "test-slug", status: ItemStatus.Published, visibility: ItemVisibility.Public);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedItem);
    }

    [Fact]
    public async Task GetItemAsync_BySlug_WhenItemNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetItemAsync(null, "missing-slug", null, null))
            .ReturnsAsync((Item?)null);

        // Act
        var act = async () => await _service.GetItemAsync(slug: "missing-slug");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetItemAsync_WithNoIdOrSlug_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _service.GetItemAsync();

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetItemsAsync_ReturnsItemsFromRepository()
    {
        // Arrange
        var maxDate = DateTimeOffset.UtcNow;
        var expectedItems = new List<Item>
        {
            CreateTestItem("Item 1"),
            CreateTestItem("Item 2"),
        };
        _mockRepository.Setup(r => r.GetItemsAsync(null, null, null, maxDate, 10, 0))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _service.GetItemsAsync(maxDate: maxDate, count: 10);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public async Task GetItemsAsync_WithStatusFilter_ReturnsFilteredItems()
    {
        // Arrange
        var expectedItems = new List<Item>
        {
            CreateTestItem("Published 1"),
            CreateTestItem("Published 2"),
        };
        _mockRepository.Setup(r => r.GetItemsAsync(ItemStatus.Published, ItemVisibility.Public, null, null, 50, 0))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _service.GetItemsAsync(status: ItemStatus.Published, visibility: ItemVisibility.Public, count: 50);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public async Task AddItemAsync_CallsRepository()
    {
        // Arrange
        var item = CreateTestItem("New Item");
        _mockRepository.Setup(r => r.AddItemAsync(item))
            .ReturnsAsync(item);

        // Act
        var result = await _service.AddItemAsync(item);

        // Assert
        result.Should().Be(item);
        _mockRepository.Verify(r => r.AddItemAsync(item), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_WhenItemExists_UpdatesItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = CreateTestItem("Original");
        var updateItem = CreateTestItem("Updated");

        _mockRepository.Setup(r => r.GetItemAsync(itemId, null, null, null))
            .ReturnsAsync(existingItem);
        _mockRepository.Setup(r => r.UpdateItemAsync(itemId, updateItem))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _service.UpdateItemAsync(itemId, updateItem);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.UpdateItemAsync(itemId, updateItem), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_WhenItemNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateItem = CreateTestItem("Updated");

        _mockRepository.Setup(r => r.GetItemAsync(itemId, null, null, null))
            .ReturnsAsync((Item?)null);

        // Act
        var act = async () => await _service.UpdateItemAsync(itemId, updateItem);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _mockRepository.Verify(r => r.UpdateItemAsync(It.IsAny<Guid>(), It.IsAny<Item>()), Times.Never);
    }

    [Fact]
    public async Task DeleteItemAsync_WhenItemExists_DeletesItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = CreateTestItem("Item to Delete");

        _mockRepository.Setup(r => r.GetItemAsync(itemId, null, null, null))
            .ReturnsAsync(existingItem);
        _mockRepository.Setup(r => r.DeleteItemAsync(itemId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteItemAsync(itemId);

        // Assert
        _mockRepository.Verify(r => r.DeleteItemAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_WhenItemNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetItemAsync(itemId, null, null, null))
            .ReturnsAsync((Item?)null);

        // Act
        var act = async () => await _service.DeleteItemAsync(itemId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _mockRepository.Verify(r => r.DeleteItemAsync(It.IsAny<Guid>()), Times.Never);
    }
}
