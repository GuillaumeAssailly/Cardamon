using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    Data_LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Data_Email = table.Column<string>(type: "TEXT", nullable: false),
                    Data_Username = table.Column<string>(type: "TEXT", nullable: false),
                    Data_BirthDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Data_Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
