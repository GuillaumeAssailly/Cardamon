using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Text;


namespace PokéService.Entities
{
    public enum Categories
    {
        Pokémon = 0,
        Energy = 1,
        Trainer = 2
    }

    public class CardsCollection
    {
        [Key]
        public required int Id { get; set; }

        public required bool IsPublic { get; set; } = false;

        public virtual string? Cards { get; set; }  // Ids of the cards joined by \n

        [NotMapped]
        public virtual string[]? CardsArray { get => Cards?.Split('\n'); }

        public bool AddCard(string cardId)
        {
            if (Cards == null) Cards = cardId;

            else if (!Cards.EndsWith(cardId) && !Cards.Contains(cardId + '\n'))
            {
                Cards += "\n" + cardId;
            }
            else return false;
            
            return true;
        }

        public bool RemoveCard(string cardId)
        {
            if (Cards == null)
            {
                return false;
            }
            else if (Cards.EndsWith(cardId))
            {
                Cards = Cards[..^cardId.Length];
                if (Cards == "") Cards = null;
                return true;
            }
            else
            {
                cardId += '\n';
                bool r = Cards.Contains(cardId);
                if (r) Cards = Cards.Replace(cardId, "");

                return r;
            }
        }
    }


    public class Pokémon
    {
        // Mandatory
        [Key]
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


        // Pokémon Category Mandatory

        public string? Types { get; set; } // e.g.: "Feu\nEau"

        [NotMapped]
        public string[]? TypesArray { get => Types?.Split('\n'); }

        [NotMapped]
        public bool Possessed { get; set; } = false; // true if the card is in the user's collection (attr attached through API)


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
    
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("Id: " + Id + "\n");
            sb.Append("Category: " + Category + "\n");
            sb.Append("Name: " + Name + "\n");
            sb.Append("Image: " + Image + "\n");
            sb.Append("SetImage: " + SetImage + "\n");

            sb.Append("Set: " + Set + "\n");
            sb.Append("LocalId: " + LocalId + "\n");
            sb.Append("Total: " + Total + "\n");
            sb.Append("Rarity: " + Rarity + "\n");
            sb.Append("Illustrator: " + Illustrator + "\n");

            sb.Append("Types: " + Types + "\n");
            sb.Append("Description: " + Description + "\n");
            sb.Append("Suffix: " + Suffix + "\n");
            sb.Append("Stage: " + Stage + "\n");
            sb.Append("EvolveFrom: " + EvolveFrom + "\n");

            sb.Append("RegulationMark: " + RegulationMark + "\n");
            sb.Append("Hp: " + Hp + "\n");
            sb.Append("Retreat: " + Retreat + "\n");
            sb.Append("Weaknesses: " + Weaknesses + "\n");
            sb.Append("Resistances: " + Resistances + "\n");

            sb.Append("Abilities: " + Abilities + "\n");
            sb.Append("Attacks: " + Attacks + "\n");
            sb.Append("Item: " + Item + "\n");
            sb.Append("EnergyType: " + EnergyType + "\n");
            sb.Append("Effect: " + Effect + "\n");

            sb.Append("TrainerType: " + TrainerType + "\n");
            return sb.ToString();
        } 
    }

    // Link classes
    /*
    public class PokémonWeakness
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public required string PokémonId { get; set; }

        [ForeignKey("PokémonId")]
        public Pokémon Pokémon { get; set; } = default!;

        public required Int64 WeaknessId { get; set; }

        [ForeignKey("WeaknessId")]
        public Weakness Weakness { get; set; } = default!;

    }

    public class PokémonResistance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public required string PokémonId { get; set; }

        [ForeignKey("PokémonId")]
        public Pokémon Pokémon { get; set; } = default!;

        public required Int64 ResistanceId { get; set; }

        [ForeignKey("ResistanceId")]
        public Resistance Resistance { get; set; } = default!;
    }

    public class PokémonAbility
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public required string PokémonId { get; set; }

        [ForeignKey("PokémonId")]
        public Pokémon Pokémon { get; set; } = default!;

        public required Int64 AbilityId { get; set; }

        [ForeignKey("AbilityId")]
        public Ability Ability { get; set; } = default!;
    }

    public class PokémonAttack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public required string PokémonId { get; set; }

        [ForeignKey("PokémonId")]
        public Pokémon Pokémon { get; set; } = default!;

        public required Int64 AttackId { get; set; }

        [ForeignKey("AttackId")]
        public Attack Attack { get; set; } = default!;
    }

    public class PokémonItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public required string PokémonId { get; set; }

        [ForeignKey("PokémonId")]
        public Pokémon Pokémon { get; set; } = default!;

        public required Int64 ItemId { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; } = default!;
    }*/


    // Data classes

    [Owned]
    public class Item
    {
        /*[Key]
        public Int64 Id { get; set; }*/

        public required string Name { get; set; }

        public required string Effect { get; set; }
    }

    [Owned]
    public class Resistance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public required string Type { get; set; }

        public string? Value { get; set; }  // "-20", "x2"...
    }

    [Owned]
    public class Weakness
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public required string Type { get; set; }

        public string? Value { get; set; }  // "-20", "x2"...
    }

    [Owned]
    public class Attack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public string? Name { get; set; }
        public string? Cost { get; set; }  // e.g.: "Feu\nEau\nEau"...
        public string? Damage { get; set; }  // "20", "30", "+20"...
        public string? Effect { get; set; }  // the text

        [NotMapped]
        public string[]? Costs { get => Cost?.Split('\n'); }
    }

    [Owned]
    public class Ability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string? Name { get; set; }
        public string? Effect { get; set; }
        public required string Type { get; set; }  // e.g.: "Talent"
    }
}
