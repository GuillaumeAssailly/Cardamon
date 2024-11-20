using Microsoft.EntityFrameworkCore;
using Common.PokéEntities;

namespace PokéService.Data
{
    public class PokéServiceContext: DbContext
    {
        public PokéServiceContext(DbContextOptions<PokéServiceContext> options)
            : base(options)
        {}

        public DbSet<Pokémon> Pokémon { get; set; } = default!;
        public DbSet<CardsCollection> CardsCollection { get; set; } = default!;

    }
}
