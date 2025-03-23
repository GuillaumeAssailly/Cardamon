using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokéService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardsCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cards = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardsCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pokémon",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Image = table.Column<string>(type: "TEXT", nullable: false),
                    SetImage = table.Column<string>(type: "TEXT", nullable: false),
                    Set = table.Column<string>(type: "TEXT", nullable: false),
                    LocalId = table.Column<string>(type: "TEXT", nullable: false),
                    Total = table.Column<int>(type: "INTEGER", nullable: false),
                    Rarity = table.Column<string>(type: "TEXT", nullable: false),
                    Illustrator = table.Column<string>(type: "TEXT", nullable: false),
                    Types = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Suffix = table.Column<string>(type: "TEXT", nullable: true),
                    Stage = table.Column<string>(type: "TEXT", nullable: true),
                    EvolveFrom = table.Column<string>(type: "TEXT", nullable: true),
                    RegulationMark = table.Column<string>(type: "TEXT", nullable: true),
                    Hp = table.Column<int>(type: "INTEGER", nullable: true),
                    Retreat = table.Column<int>(type: "INTEGER", nullable: true),
                    Item_Name = table.Column<string>(type: "TEXT", nullable: true),
                    Item_Effect = table.Column<string>(type: "TEXT", nullable: true),
                    EnergyType = table.Column<string>(type: "TEXT", nullable: true),
                    Effect = table.Column<string>(type: "TEXT", nullable: true),
                    TrainerType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokémon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ability",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Effect = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    PokémonId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ability_Pokémon_PokémonId",
                        column: x => x.PokémonId,
                        principalTable: "Pokémon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attack",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Cost = table.Column<string>(type: "TEXT", nullable: true),
                    Damage = table.Column<string>(type: "TEXT", nullable: true),
                    Effect = table.Column<string>(type: "TEXT", nullable: true),
                    PokémonId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attack_Pokémon_PokémonId",
                        column: x => x.PokémonId,
                        principalTable: "Pokémon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resistance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    PokémonId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resistance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resistance_Pokémon_PokémonId",
                        column: x => x.PokémonId,
                        principalTable: "Pokémon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weakness",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    PokémonId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weakness", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weakness_Pokémon_PokémonId",
                        column: x => x.PokémonId,
                        principalTable: "Pokémon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ability_PokémonId",
                table: "Ability",
                column: "PokémonId");

            migrationBuilder.CreateIndex(
                name: "IX_Attack_PokémonId",
                table: "Attack",
                column: "PokémonId");

            migrationBuilder.CreateIndex(
                name: "IX_Resistance_PokémonId",
                table: "Resistance",
                column: "PokémonId");

            migrationBuilder.CreateIndex(
                name: "IX_Weakness_PokémonId",
                table: "Weakness",
                column: "PokémonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ability");

            migrationBuilder.DropTable(
                name: "Attack");

            migrationBuilder.DropTable(
                name: "CardsCollection");

            migrationBuilder.DropTable(
                name: "Resistance");

            migrationBuilder.DropTable(
                name: "Weakness");

            migrationBuilder.DropTable(
                name: "Pokémon");
        }
    }
}
