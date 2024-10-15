using System.ComponentModel.DataAnnotations.Schema;


namespace Front.Entities
{
    public enum Categories
    {
        Pokémon = 0,
        Energy = 1,
        Trainer = 2
    }


    public class Pokémon
    {
        // Mandatory
        public required string Id { get; set; }

        public required Categories Category { get; set; }

        public required string Name { get; set; }

        public required string Image { get; set; } // url

        public required string SetImage { get; set; } // set url

        public required string Set { get; set; }  // Set name: e.g.: "Sword & Shield"

        public required string LocalId { get; set; } // the X of X / Y (card No in the set), e.g.: 1, 5, "24a"

        public required int Total { get; set; } // the Y of X / Y (card Num of the set)

        public required string Rarity { get; set; }

        public required string Illustrator { get; set; }

        [NotMapped]
        public bool Possessed { get; set; } = false; // true if the card is in the user's collection

        // Pokémon Category Mandatory

        public string? Types { get; set; } // e.g.: "Feu\nEau"

        [NotMapped]
        public string[]? TypesArray { get => Types?.Split('\n'); }


        // Pokémon Category Optional

        public string? Description { get; set; }

        public string? Suffix { get; set; } // e.g.: "Mega", "Primo"

        public string? Stage { get; set; } // e.g.: "Stage 1"

        public string? EvolveFrom { get; set; } // e.g.: "Salameche"

        public string? RegulationMark { get; set; } // e.g.: "E", "D", "F", "G"

        public int? Hp { get; set; } // e.g.: 70

        public int? Retreat { get; set; } // e.g.: 0

        // Foreign Keys

        public List<Weakness>? Weaknesses { get; set; }

        public List<Resistance>? Resistances { get; set; }

        public List<Ability>? Abilities { get; set; }

        public List<Attack>? Attacks { get; set; }

        public Item? Item { get; set; }


        // Energy Category Mandatory

        /// None

        // Energy Category Optional

        public string? EnergyType { get; set; } // e.g.: "Standard", "Spécial"

        public string? Effect { get; set; }

        /// + {'regulationMark', 'types', 'stage', 'effect', 'hp'}

        // Trainer Category Mandatory

        /// None

        // Trainer Category Optional
        public string? TrainerType { get; set; } // e.g.: "Outil"

        /// + {'regulationMark', 'types', 'stage', 'retreat', 'weaknesses', 'abilities', 'trainerType', 'energyType', 'attacks', 'effect', 'suffix', 'hp'}
    }

    // Data classes

    public class Item
    {
        public required string Name { get; set; }
        public required string Effect { get; set; }
    }

    public class Resistance
    {
        public Int64 Id { get; set; }

        public required string Type { get; set; }

        public string? Value { get; set; }  // "-20", "x2"...
    }

    public class Weakness
    {
        public Int64 Id { get; set; }

        public required string Type { get; set; }

        public string? Value { get; set; }  // "-20", "x2"...
    }

    public class Attack
    {
        public Int64 Id { get; set; }
        public string? Name { get; set; }
        public string? Cost { get; set; }  // e.g.: "Feu\nEau\nEau"...
        public string? Damage { get; set; }  // "20", "30", "+20"...
        public string? Effect { get; set; }  // the text

        [NotMapped]
        public string[]? Costs { get => Cost?.Split('\n'); }
    }

    public class Ability
    {
        public Int64 Id { get; set; }

        public string? Name { get; set; }
        public string? Effect { get; set; }
        public required string Type { get; set; }  // e.g.: "Talent"
    }
}
