using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PokéService.Entities;
using System.Net;
using System.Text;

namespace GatewayService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PokéController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public PokéController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Collection: api/Poké/collection



        // GET: api/Poké/collection/public
        [Authorize]
        [HttpGet("collection/public")]
        public async Task<ActionResult<bool>> IsPublic() // is collection public
        {
            var id = GetLoggedId();
            if (id is null) return Unauthorized();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

                HttpResponseMessage response = await client.GetAsync($"api/Poké/collection/public/{id}");

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok(await response.Content.ReadFromJsonAsync<bool>());
                else return NotFound(await response.Content.ReadAsStringAsync());
            }
        }


        // POST: api/Poké/collection/public
        [Authorize]
        [HttpPost("collection/public/{isPublic}")]
        public async Task<ActionResult> SetProtection(bool isPublic) // set collection protection
        {
            var id = GetLoggedId();
            if (id is null) return Unauthorized();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

				HttpResponseMessage response = await client.PostAsync($"api/Poké/collection/public/{id}/{isPublic}", null);

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok();
                else return NotFound(await response.Content.ReadAsStringAsync());
            }
        }


        // GET: api/Poké/collection
        [Authorize]
        [HttpGet("collection")]
        public async Task<ActionResult<string[]>> GetCollection() // get all cards ids
        {
            var id = GetLoggedId();
            if (id is null) return Unauthorized();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

                HttpResponseMessage response = await client.GetAsync($"api/Poké/collection/{id}");

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok(await response.Content.ReadFromJsonAsync<string[]>());
                else return NotFound(await response.Content.ReadAsStringAsync());
            }
        }


        // GET: api/Poké/collection/from/uname?name=&set=&category=&lid=&type1=&type2=&hp=&illustrator=&limit=&offset=
        [HttpGet("collection/from/{uname}")]
        public async Task<ActionResult<IEnumerable<Pokémon>>> GetPokémonCollectionFrom( // get cards data from a user pseudo
            string uname,
            string? name = null,
            string? set = null,
            Categories? category = null,
            string? lid = null,
            string? type1 = null,
            string? type2 = null,
            string? illustrator = null,
            int? hp = null,
            int limit = 50,
            int offset = 0)
        {
            var id = GetLoggedId();
            int uid = 0;


            /// Get user id from pseudo
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                HttpResponseMessage response = await client.GetAsync($"api/Users/name/{uname}");

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) uid = await response.Content.ReadFromJsonAsync<int>();
                else return NotFound();
            }

			using (var client = _httpClientFactory.CreateClient()) // check if the collection is public
			{
				client.BaseAddress = new System.Uri("http://localhost:5002/");

				HttpResponseMessage response = await client.GetAsync($"api/Poké/collection/public/{uid}");
                ;
				if (!response.IsSuccessStatusCode || !(await response.Content.ReadFromJsonAsync<bool>())) return NotFound();
			}

			using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

                // Generate query string in a stringbuilder
                StringBuilder query = new StringBuilder();
                if (!String.IsNullOrEmpty(name)) query.Append($"name={name}&");
                if (!String.IsNullOrEmpty(set)) query.Append($"set={set}&");
                if (category is not null) query.Append($"category={category}&");
                if (!String.IsNullOrEmpty(lid)) query.Append($"lid={lid}&");
                if (!String.IsNullOrEmpty(type1)) query.Append($"type1={type1}&");
                if (!String.IsNullOrEmpty(type2)) query.Append($"type2={type2}&");
                if (!String.IsNullOrEmpty(illustrator)) query.Append($"illustrator={illustrator}&");
                if (hp is not null) query.Append($"hp={hp}&");
                query.Append($"limit={limit}&");
                query.Append($"offset={offset}&");
                if (id is not null) query.Append($"id={id}&");

                client.BaseAddress = new System.Uri("http://localhost:5002/");

                HttpResponseMessage response = await client.GetAsync($"api/Poké/collection/{uid}/card?{query}");

				// Check if the response status code is 200 (OK)
				if (response.IsSuccessStatusCode) return Ok(await response.Content.ReadFromJsonAsync<IEnumerable<Pokémon>>());
                else return NotFound(await response.Content.ReadAsStringAsync());
                // avoid deserialize+reserialize ?? -> return Content(await apiResponse.Content.ReadAsStringAsync(), apiResponse.Content.Headers.ContentType.MediaType);
            }
        }


        // GET: api/Poké/collection/card?name=&set=&category=&lid=&type1=&type2=&hp=&illustrator=&limit=&offset=
        [Authorize]
        [HttpGet("collection/card")]
        public async Task<ActionResult<IEnumerable<Pokémon>>> GetPokémonCollection( // get cards data
            string? name = null,
            string? set = null,
            Categories? category = null,
            string? lid = null,
            string? type1 = null,
            string? type2 = null,
            string? illustrator = null,
            int? hp = null,
            int limit = 50,
            int offset = 0)
        {
            var id = GetLoggedId();
            if (id is null) return Unauthorized();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

                // generate query string in a stringbuilder
                StringBuilder query = new StringBuilder();
                if (!String.IsNullOrEmpty(name)) query.Append($"name={name}&");
                if (!String.IsNullOrEmpty(set)) query.Append($"set={set}&");
                if (category is not null) query.Append($"category={category}&");
                if (!String.IsNullOrEmpty(lid)) query.Append($"lid={lid}&");
                if (!String.IsNullOrEmpty(type1)) query.Append($"type1={type1}&");
                if (!String.IsNullOrEmpty(type2)) query.Append($"type2={type2}&");
                if (!String.IsNullOrEmpty(illustrator)) query.Append($"illustrator={illustrator}&");
                if (hp is not null) query.Append($"hp={hp}&");
                query.Append($"limit={limit}&");
                query.Append($"offset={offset}&");

                client.BaseAddress = new System.Uri("http://localhost:5002/");

                HttpResponseMessage response = await client.GetAsync($"api/Poké/collection/{id}/card?{query}"); // fast forward query arguments

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok(await response.Content.ReadFromJsonAsync<IEnumerable<Pokémon>>());
                else return NotFound(await response.Content.ReadAsStringAsync());
                // avoid deserialize+reserialize ?? -> return Content(await apiResponse.Content.ReadAsStringAsync(), apiResponse.Content.Headers.ContentType.MediaType);
            }
        }


        // POST: api/Poké/collection/card/1
        [Authorize]
        [HttpPost("collection/card/{cid}")]
        public async Task<IActionResult> AddCard(string cid) // add card to collection
        {
            var id = GetLoggedId();
            if (id is null) return Unauthorized();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

                HttpResponseMessage response = await client.PostAsync($"api/Poké/collection/{id}/card/{cid}", null);

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok();
                else if (response.StatusCode == HttpStatusCode.NotFound) return NotFound(await response.Content.ReadAsStringAsync());
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
        }

        // DELETE: api/Poké/collection/card/1
        [Authorize]
        [HttpDelete("collection/card/{cid}")]
        public async Task<IActionResult> DeleteCard(string cid) // delete card from collection
        {
            var id = GetLoggedId();
            if (id is null) return Unauthorized();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

                HttpResponseMessage response = await client.DeleteAsync($"api/Poké/collection/{id}/card/{cid}");

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok();
                return NotFound(await response.Content.ReadAsStringAsync());
            }
        }



        // Pokémon: api/Poké/card


		// GET: api/Poké/card/5?id=
		[HttpGet("card/{cid}")]
		public async Task<ActionResult<Pokémon>> GetCard(string cid)
        {
            var id = GetLoggedId();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

                string query = $"{cid}";
                if (id is not null) query += $"?id={id}";

                HttpResponseMessage response = await client.GetAsync($"api/Poké/card/{query}");

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok(await response.Content.ReadFromJsonAsync<Pokémon>());
                else return NotFound(await response.Content.ReadAsStringAsync());
                // avoid deserialize+reserialize ?? -> return Content(await apiResponse.Content.ReadAsStringAsync(), apiResponse.Content.Headers.ContentType.MediaType);
            }
        }


        // GET: api/Poké/search?id=&name=&set=&category=&lid=&type1=&type2=&hp=&illustrator=&limit=&offset=
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Pokémon>>> SearchCard(
			string? name = null,
            string? set = null,
            Categories? category = null,
            string? lid = null,
            string? type1 = null,
            string? type2 = null,
            string? illustrator = null,
            int? hp = null,
            int limit = 50,
            int offset = 0)
        {
            var id = GetLoggedId();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                var arg = this.HttpContext.Request.QueryString;

                client.BaseAddress = new System.Uri("http://localhost:5002/");

                // generate query string in a stringbuilder
                StringBuilder query = new StringBuilder();
                if (!String.IsNullOrEmpty(name)) query.Append($"name={name}&");
                if (!String.IsNullOrEmpty(set)) query.Append($"set={set}&");
                if (category is not null) query.Append($"category={category}&");
                if (!String.IsNullOrEmpty(lid)) query.Append($"lid={lid}&");
                if (!String.IsNullOrEmpty(type1)) query.Append($"type1={type1}&");
                if (!String.IsNullOrEmpty(type2)) query.Append($"type2={type2}&");
                if (!String.IsNullOrEmpty(illustrator)) query.Append($"illustrator={illustrator}&");
                if (hp is not null) query.Append($"hp={hp}&");
                query.Append($"limit={limit}&");
                query.Append($"offset={offset}&");
                if (id is not null) query.Append($"id={id}&");


                HttpResponseMessage response = await client.GetAsync($"api/Poké/search?{query.ToString()}");

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode) return Ok(await response.Content.ReadFromJsonAsync<IEnumerable<Pokémon>>());
                else return NotFound(await response.Content.ReadAsStringAsync());
                // avoid deserialize+reserialize ?? -> return Content(await apiResponse.Content.ReadAsStringAsync(), apiResponse.Content.Headers.ContentType.MediaType);
            }
        }

        private int? GetLoggedId()
        { 
			var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
			if (UserId == null || !int.TryParse(UserId, out int id)) return null;
			return id;
		}
	}
}
