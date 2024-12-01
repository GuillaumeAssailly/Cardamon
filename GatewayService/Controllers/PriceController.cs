using Microsoft.AspNetCore.Mvc;
using Common.PriceEntities;

namespace GatewayService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public PriceController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Price: api/Price/card

		// GET: api/Price/card/5?id=
		[HttpGet("card/{cid}")]
		public async Task<ActionResult<Price>> GetPrice(string cid)
        {

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5003/");

                HttpResponseMessage response = await client.GetAsync($"api/Price/card/{cid}");

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok(await response.Content.ReadFromJsonAsync<Price>());
                else return NotFound(await response.Content.ReadAsStringAsync());
                // avoid deserialize+reserialize ?? -> return Content(await apiResponse.Content.ReadAsStringAsync(), apiResponse.Content.Headers.ContentType.MediaType);
            }
        }
	}
}
