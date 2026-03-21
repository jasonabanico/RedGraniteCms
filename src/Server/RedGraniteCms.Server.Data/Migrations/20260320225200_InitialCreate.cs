using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedGraniteCms.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<string>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<string>(type: "TEXT", nullable: true),
                    AncestorIds = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    PublishedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Visibility = table.Column<int>(type: "INTEGER", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_LastModifiedAt",
                table: "Items",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Items_OwnerId",
                table: "Items",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_OwnerId_ContentType",
                table: "Items",
                columns: new[] { "OwnerId", "ContentType" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_OwnerId_CreatedAt",
                table: "Items",
                columns: new[] { "OwnerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_Slug",
                table: "Items",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Status_Visibility",
                table: "Items",
                columns: new[] { "Status", "Visibility" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
