using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLastActiveUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql(@"
        UPDATE ""Users"" u
        SET ""LastActiveAt"" = COALESCE(
            (
                SELECT MAX(o.""OrderDate"")
                FROM ""Orders"" o
                WHERE o.""UserId"" = u.""Id""
            ),
            u.""CreatedAt""
        );
    ");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastActiveAt",
                table: "Users",
                column: "LastActiveAt",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_LastActiveAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastActiveAt",
                table: "Users");
        }
    }
}
