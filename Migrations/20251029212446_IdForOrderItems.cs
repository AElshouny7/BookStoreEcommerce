using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class IdForOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderItems");
        }
    }
}
