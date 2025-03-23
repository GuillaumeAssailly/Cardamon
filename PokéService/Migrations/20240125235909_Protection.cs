using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokéService.Migrations
{
    /// <inheritdoc />
    public partial class Protection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "CardsCollection",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "CardsCollection");
        }
    }
}
