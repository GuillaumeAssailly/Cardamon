using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using UserService.Entities;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        // POST: api/User/exists/Ash
        [HttpGet("exists/{pseudo}")]
        public async Task<ActionResult> IsExists(string pseudo)
        {
            int id;

            using (var client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                HttpResponseMessage response = await client.GetAsync($"api/Users/name/{pseudo}");

                if (!response.IsSuccessStatusCode) return NotFound();

                id = await response.Content.ReadFromJsonAsync<int>(); // user Found
            }

            using (var client = _httpClientFactory.CreateClient()) // check if the collection is public
            {
                client.BaseAddress = new System.Uri("http://localhost:5002/");

                HttpResponseMessage response = await client.GetAsync($"api/Poké/collection/public/{id}");

                if (response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>()) return Ok();
                else return NotFound();
            }
        }


        // POST: api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreate model)
        {
      
            using (var client = _httpClientFactory.CreateClient())
            {
          
                client.BaseAddress = new System.Uri("http://localhost:5001/");

      
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/register", model);

         
                if (response.IsSuccessStatusCode)
                {
                    // Account created, now create a collection for the user
                    int? user = await response.Content.ReadFromJsonAsync<int>();
                    if (user != null)
                    {
                        using (var Pokéclient = _httpClientFactory.CreateClient())
                        {
                            Pokéclient.BaseAddress = new System.Uri("http://localhost:5002/");
                            response = await Pokéclient.PostAsync($"api/Poké/collection/{user}", null);
                            if (!response.IsSuccessStatusCode) return StatusCode((int)HttpStatusCode.ServiceUnavailable, "Gateway failed to connect to the collection server");
                        }

                    } else return StatusCode((int)HttpStatusCode.InternalServerError, "Register service returned incoherent results");

                    return Created();
                }
                else return BadRequest(await response.Content.ReadAsStringAsync());
            }
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(UserLogin model)
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/login", model);

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode)
                {
                    // You can deserialize the response content here if needed
                    var result = await response.Content.ReadFromJsonAsync<UserInfo>();
                    if (result != null)
                    {
                        UserDTO user = new UserDTO()
                        {
                            Data = result.Data,
                            Token = GenerateJwtToken(result.Id)
                        };

                        return Ok(user);

                    } else return StatusCode((int)HttpStatusCode.InternalServerError, "Login service returned incoherent results");
                    
                }
                else return BadRequest(await response.Content.ReadAsStringAsync());
            }
        }

        // PUT: api/User
        [Authorize]
        [HttpPut("")]
        public async Task<ActionResult> UpdateUser(UserData model)
        {
            var id = GetLoggedId();
            if (id is null) return Unauthorized();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PutAsJsonAsync($"api/Users/{id}", model);

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(await response.Content.ReadAsStringAsync());
                }
            }
        }


        // PUT: api/User
        [Authorize]
        [HttpPut("pass")]
        public async Task<ActionResult> UpdatePasswordUser(UserPasswordChange model)
        {
            var id = GetLoggedId();
            if (id is null) return Unauthorized();

            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PutAsJsonAsync($"api/Users/pass/{id}", model);

                // Check if the response status code is 200 (OK)
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(await response.Content.ReadAsStringAsync());
                }
            }
        }

        // DELETE: api/User
        [Authorize]
        [HttpDelete("")]
        public async Task<IActionResult> Delete()
		{
			var id = GetLoggedId();
            if (id is null) return Unauthorized();

            using (var client = _httpClientFactory.CreateClient())
            {

                client.BaseAddress = new System.Uri("http://localhost:5001/");


                HttpResponseMessage response = await client.DeleteAsync($"api/Users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    using (var Pokéclient = _httpClientFactory.CreateClient())
                    {
                        Pokéclient.BaseAddress = new System.Uri("http://localhost:5002/");

                        response = await Pokéclient.DeleteAsync($"api/Poké/collection/{id}");
                        if (!response.IsSuccessStatusCode) return StatusCode((int)HttpStatusCode.ServiceUnavailable, "Gateway failed to connect to the collection server");
                        return Ok();
                    }
                }
                else return BadRequest(await response.Content.ReadAsStringAsync());
            }
        }


        private string GenerateJwtToken(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString()) // add the user id as a claim
            };

            // create a key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Fun fact: Ash's Pikachu name is Jean-Luc"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // token configuration
            var token = new JwtSecurityToken(
                issuer: "Cardamon", // Who created this token
                audience: "localhost:5000", // Who can use this token
                claims: claims, // data inside the token
                expires: DateTime.Now.AddMinutes(3000), // validity of the token
                signingCredentials: creds); // key to encrypt the token

            // return the token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int? GetLoggedId()
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null || !int.TryParse(UserId, out int id)) return null;
			return id;
        }
    }
}
