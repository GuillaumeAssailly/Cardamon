using Common.PriceEntities;


namespace Front.Services
{
    public class PriceService
    {
        private readonly AuthService auth;
        public PriceService(AuthService auth)
        {
            this.auth = auth;
        }


        public async Task<Price?> GetPrice(string id)
		{
			var res = await auth._httpClient.GetAsync($"api/Price/card/{id}");

            if (res.IsSuccessStatusCode) return await res.Content.ReadFromJsonAsync<Price>();
            return null;
        }
    }
}


