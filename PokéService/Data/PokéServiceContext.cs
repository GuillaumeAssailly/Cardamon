using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PokéService.Entities;

namespace PokéService.Data
{
    public class PokéServiceContext: DbContext
    {
        public PokéServiceContext(DbContextOptions<PokéServiceContext> options)
            : base(options)
        {
        }

        public DbSet<Pokémon> Pokémon { get; set; } = default!;
        public DbSet<CardsCollection> CardsCollection { get; set; } = default!;

    }
}
