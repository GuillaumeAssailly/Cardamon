using Common.PokéEntities;


namespace Front.Services
{
    public class PokémonService
    {
        private readonly AuthService auth;
        public PokémonService(AuthService auth)
        {
            this.auth = auth;
        }


        public async Task<Pokémon?> GetPokémon(string id)
		{
			var res = await auth._httpClient.GetAsync($"api/Poké/card/{id}");

            if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<Pokémon>();
            return null;
        }

        public async Task<IEnumerable<Pokémon>?> Search(string query = "")
		{
			var res = await auth._httpClient.GetAsync($"api/Poké/search{query}");

            if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<IEnumerable<Pokémon>>();
            return null;
        }

        public async Task<IEnumerable<Pokémon>?> GetCollection(string query = "", string? pseudo = null)
        {
			var res = (pseudo is null) ?
				await auth._httpClient.GetAsync($"api/Poké/collection/card{query}"):
			    await auth._httpClient.GetAsync($"api/Poké/collection/from/{pseudo}{query}");

			if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<IEnumerable<Pokémon>>();
            return null;
        }

        public async Task<bool> IsExists(string name)
        {
            var res = await auth._httpClient.GetAsync($"api/User/exists/{name}");

            return res.IsSuccessStatusCode;
        }

        public async Task<bool> IsPublic()
        {
            var res = await auth._httpClient.GetAsync($"api/Poké/collection/public");

            return res.IsSuccessStatusCode && await res.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> SetProtection(bool isPublic)
		{
			var res = await auth._httpClient.PostAsync($"api/Poké/collection/public/{isPublic}", null);
			return res.IsSuccessStatusCode;
		}

		public async Task<bool> AddCard(string cid)
		{
			var res = await auth._httpClient.PostAsync($"api/Poké/collection/card/{cid}", null);

			return res.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteCard(string cid)
        {
            var res = await auth._httpClient.DeleteAsync($"api/Poké/collection/card/{cid}");

            return res.IsSuccessStatusCode;
        }

        public string[] GetTypes()
        {
            return new string[] { "Tous", "Incolore", "Plante", "Eau", "Feu", "Électrique", "Métal", "Psy", "Combat", "Obscurité", "Dragon", "Fée" };
        }

        public string GetTypeClass(string type)
        {
            return type switch
            {
                "Tous" => "energy-all",
                "Plante" => "energy-grass",
                "Feu" => "energy-fire",
                "Eau" => "energy-water",
                "Psy" => "energy-psychic",
                "Combat" => "energy-fighting",
                "Obscurité" => "energy-darkness",
                "Métal" => "energy-metal",
                "Fée" => "energy-fairy",
                "Dragon" => "energy-dragon",
                "Électrique" => "energy-lightning",
                "Incolore" => "energy-colorless",
                _ => "energy-colorless",
            };
        }

        // may be improved...
        public string[] GetCategories()
        {
            return new string[] { "Toutes les cartes", "Cartes Pokémon", "Cartes Énergies", "Cartes Dresseurs" };
        }

        public string GetCategoryClass(string cat)
        {
            return cat switch
            {
                "Cartes Pokémon" => "pkmn-logo-pkmn",
                "Cartes Énergies" => "pkmn-logo-energy",
                "Cartes Dresseurs" => "pkmn-logo-trainer",
                _ => "pkmn-logo-all",
            };
        }
        public string GetCategoryValue(string cat)
        {
            return cat switch
            {
                "Cartes Pokémon" => "0",
                "Cartes Énergies" => "1",
                "Cartes Dresseurs" => "2",
                _ => "",
            };
        }
        public string GetCategoryText(string cat)
        {
            return cat switch
            {
                 "0" => "Cartes Pokémon",
                "1" => "Cartes Énergies",
                "2" => "Cartes Dresseurs",
                _ => "Toutes les cartes",
            };
        }
    }
}


