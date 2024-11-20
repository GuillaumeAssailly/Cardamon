using Common.UserEntities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace Front.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private ProtectedLocalStorage _sessionStorage;
        
        public string? JWToken { get; private set; } = null;

		public event Action JWTokenUpdate;

        public CustomAuthenticationStateProvider(ProtectedLocalStorage protectedSessionStorage)
        {
            _sessionStorage = protectedSessionStorage;
			JWTokenUpdate += () => Console.WriteLine("JWTokenUpdate triggered");
		}


        public async Task<ClaimsPrincipal> MarkUserAsAuthenticated(UserDTO user)
        {
            JWToken = user.Token;
            JWTokenUpdate?.Invoke();
            await _sessionStorage.SetAsync("User", user);
            var claims = new[] {
                new Claim(ClaimTypes.Name, user.Data.Username),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            _currentUser = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return _currentUser;
		}
		public async Task<ClaimsPrincipal> Logout()
		{
			JWToken = null;
			JWTokenUpdate?.Invoke();
			await _sessionStorage.DeleteAsync("User");
			_currentUser = new ClaimsPrincipal(new ClaimsIdentity());

			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
			return _currentUser;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			ProtectedBrowserStorageResult<UserDTO>? userSession = null;

            try
            {
                userSession = await _sessionStorage.GetAsync<UserDTO>("User");
            }
            catch
            {
                Console.WriteLine("Error while getting user session");
            }

            if (userSession is not null && userSession.Value.Success && userSession.Value.Value != null)
            {
                var user = userSession.Value.Value;
                JWToken = user.Token; // update token
				JWTokenUpdate?.Invoke();
				var claims = new[] {
                    new Claim(ClaimTypes.Name, user.Data.Username),
                    new Claim(ClaimTypes.Role, "User")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                _currentUser = new ClaimsPrincipal(identity);
			} else
			{
				_currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
            return await Task.FromResult(new AuthenticationState(_currentUser));
        }

        public async Task<UserData?> GetUserDataAsync()
        {
            var userSession = await _sessionStorage.GetAsync<UserDTO>("User");
            if (userSession.Success && userSession.Value != null) return userSession.Value.Data;
            return null;
        }

        public async Task SetUserDataAsync(UserData ud)
        {
            var userSession = await _sessionStorage.GetAsync<UserDTO>("User");
            if (userSession.Success && userSession.Value != null)
            {
                userSession.Value.Data = ud;
                await MarkUserAsAuthenticated(userSession.Value);
            }
        }
    }
}
