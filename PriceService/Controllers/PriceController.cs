using Common.PriceEntities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PriceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PriceController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        // GET: api/Price/card/5
        [HttpGet("card/{id}")]
        public async Task<ActionResult<Price>> GetPrice(string id) // try to get the card price
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("https://api.pokemontcg.io/");

                HttpResponseMessage response = await client.GetAsync($"v2/cards/{id}");

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode)
                {
                    var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("error", out var error))
                    {
                        return NotFound("card not found");
                    }

                    Price price = new Price();

                    if (root.TryGetProperty("data", out var data))
                    {
                        if (data.TryGetProperty("tcgplayer", out var tcgplayer))
                        {
                            if (tcgplayer.TryGetProperty("url", out var url))
                            {
                                price.TCGPlayerUrl = url.GetString();
                            }

                            if (tcgplayer.TryGetProperty("prices", out var prices))
                            {
                                if (prices.TryGetProperty("normal", out var normal))
                                {
                                    if (normal.TryGetProperty("market", out var market))
                                    {
                                        try
                                        {
                                            price.TCGPlayerPrice = market.GetDouble();
                                        }
                                        catch (Exception) { }
                                    }
                                }
                            }
                        }

                        if (data.TryGetProperty("cardmarket", out var cardmarket))
                        {
                            if (cardmarket.TryGetProperty("url", out var url))
                            {
                                price.CardMarketUrl = url.GetString();
                            }

                            if (cardmarket.TryGetProperty("prices", out var prices))
                            {
                                if (prices.TryGetProperty("averageSellPrice", out var averageSellPrice))
                                {
                                    try
                                    {
                                        price.CardMarketPrice = averageSellPrice.GetDouble();
                                    }
                                    catch (Exception) { }
                                }
                            }
                        }
                    }

                    /*
                     root['data']['cardmarket']['url']
                     root['data']['cardmarket']['prices']['averageSellPrice']

                     root['data']['tcgplayer']['url']
                     root['data']['tcgplayer']['prices']["normal"]["market"]
                    */

                    if ((price.CardMarketUrl != null && price.CardMarketPrice != null) || 
                        (price.TCGPlayerUrl != null && price.TCGPlayerPrice != null)) 
                    {
                        return Ok(price);
                    }
                    return NotFound("empty data");
                }
                else return NotFound(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
