using Common.UserEntities;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;


namespace Front.Services
{
    /// <summary>
    /// Base class for all services that need to be authenticated (that requests JWT protected API)
    /// </summary>
    public class AuthService
    {
        public readonly HttpClient _httpClient;
        protected readonly CustomAuthenticationStateProvider _auth;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider customAuthenticationStateProvider
            )
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new System.Uri("http://localhost:5000/");
            _auth = (CustomAuthenticationStateProvider)customAuthenticationStateProvider;
            _auth.JWTokenUpdate += UpdateJWToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _auth.JWToken);
        }

        private void UpdateJWToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _auth.JWToken);
        }

		public async Task<UserData?> GetUserDataAsync()
		{
			return await _auth.GetUserDataAsync();
		}
		public async Task SetUserDataAsync(UserData u)
		{
			await _auth.SetUserDataAsync(u);
		}

        public async Task Logout()
        {
			await _auth.Logout();
		}
	}
}


