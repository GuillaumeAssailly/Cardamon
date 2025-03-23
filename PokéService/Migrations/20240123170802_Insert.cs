using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;
using PokéService.Data;
using Common.PokéEntities;

#nullable disable

namespace PokéService.Migrations
{
	/// <inheritdoc />
	public partial class Insert : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{

			var context = new PokéServiceContext(
							   new DbContextOptionsBuilder<PokéServiceContext>()
												  .UseSqlite("Data Source=Poké.db")
																	 .Options);


			Console.Write("Base ");
			Console.WriteLine(context.Pokémon.ToList().Count);

			// Cards
			Console.WriteLine("Load cards...");
			var clist = JsonConvert.DeserializeObject<List<Pokémon>>(File.ReadAllText("Migrations/pkmn.all.json"));
			Console.WriteLine("Insert cards...");

			int i = 0;
			foreach (var dto in clist)
			{
				Console.WriteLine($"Insert card {i++}...");
				context.Pokémon.Add(dto);
			}

			Console.WriteLine("Saving...");

			context.SaveChanges();

			Console.WriteLine("Done!");

			Console.WriteLine(context.Pokémon.FirstOrDefault(p => p.Name == "xy9-3"));
			Console.WriteLine(context.Pokémon.Select(p => p.Name == "Pikachu").ToList().Count);
		}


		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DELETE FROM Pokémon");
			migrationBuilder.Sql("DELETE FROM Item");
			migrationBuilder.Sql("DELETE FROM Weakness");
			migrationBuilder.Sql("DELETE FROM Resistance");
			migrationBuilder.Sql("DELETE FROM Attack");
			migrationBuilder.Sql("DELETE FROM Ability");
			migrationBuilder.Sql("DELETE FROM CardsCollection");
		}
	}
}
