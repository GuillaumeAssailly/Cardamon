using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using Common.UserEntities;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserServiceContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UsersController(UserServiceContext context, PasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.User.ToListAsync();
        }

        // DELETE: api/Users
        [HttpDelete]
        public async Task<ActionResult> DeleteAllUsers()
        {
            foreach (var c in _context.User) _context.User.Remove(c);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserData>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Data);
        }

        // GET: api/Users/name/Ash
        [HttpGet("name/{name}")]
        public async Task<ActionResult<int>> GetId(string name)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Data.Username == name);

            if (user == null) return NotFound();

            return Ok(user.Id);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserData userUdpate)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null) return NotFound();

            var emailValidationAttribute = new EmailAddressAttribute();


            if (!userUdpate.Email.IsNullOrEmpty())
            {
                if (!emailValidationAttribute.IsValid(userUdpate.Email))
                    return BadRequest("E2: Error, email is invalid");
                else
                {
                    var check = await _context.User.FirstOrDefaultAsync(u => u.Data.Email == userUdpate.Email);
                    if (check != null && check.Id != user.Id)
                        return BadRequest("E3: Error, email already assigned to another account");
                }
            }

            if (!userUdpate.Username.IsNullOrEmpty())
            {
                if (emailValidationAttribute.IsValid(userUdpate.Username))
                    return BadRequest("E5: Error, username cannot be an email");
                else if (userUdpate.Username.Length < 6)
                    return BadRequest("E7: Error, username must be at least 6 characters long");
                else if (userUdpate.Username.Length > 20)
                    return BadRequest("E8: Error, username must be at most 20 characters long");
                else
                {
                    var check = await _context.User.FirstOrDefaultAsync(u => u.Data.Username == userUdpate.Username);
                    if (check != null && check.Id != user.Id)
                        return BadRequest("E6: Error, username already assigned to another account");
                }
            }
            

            if (!userUdpate.FirstName.IsNullOrEmpty())
            {
                if (!userUdpate.FirstName.All(c => char.IsLetter(c)))
                    return BadRequest("E9: Error, firstname can only contain letters");
                else if (userUdpate.FirstName.Length < 1)
                    return BadRequest("E10: Error, firstname must be at least 1 characters long");
                else if (userUdpate.FirstName.Length > 40)
                    return BadRequest("E11: Error, firstname must be at most 40 characters long");
            }


            if (!userUdpate.LastName.IsNullOrEmpty())
            {
                if (!userUdpate.LastName.All(c => char.IsLetter(c)))
                    return BadRequest("E13: Error, lastname can only contain letters");
                else if (userUdpate.LastName.Length < 1)
                    return BadRequest("E14: Error, lastname must be at least 1 characters long");
                else if (userUdpate.LastName.Length > 40)
                    return BadRequest("E15: Error, lastname must be at most 40 characters long");
            }


            if (userUdpate.BirthDate.Year < 1900)
                return BadRequest("E23: Error, birthdate must be after 1900");
            else if (userUdpate.BirthDate.Year > DateTime.Now.Year - 8)
                return BadRequest($"E24: Error, birthdate must be before {DateTime.Now.Year - 8}");


            if (!userUdpate.Email.IsNullOrEmpty()) user.Data.Email = userUdpate.Email;
            if (!userUdpate.Username.IsNullOrEmpty()) user.Data.Username = userUdpate.Username;
            if (!userUdpate.FirstName.IsNullOrEmpty()) user.Data.FirstName = userUdpate.FirstName;
            if (!userUdpate.LastName.IsNullOrEmpty()) user.Data.LastName = userUdpate.LastName;

            user.Data.BirthDate = userUdpate.BirthDate;
            user.Data.Gender = userUdpate.Gender;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                else throw;
            }

            return Ok();
        }


        // PUT: api/Users/pass/5
        [HttpPut("pass/{id}")]
        public async Task<IActionResult> UpdatePasswordUser(int id, UserPasswordChange userUdpate)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null) return NotFound();


            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userUdpate.OldPass) == PasswordVerificationResult.Success)
            {
                if (userUdpate.NewPass.IsNullOrEmpty())
                    return BadRequest("E16: Error, password cannot be empty");
                else if (!userUdpate.NewPass.Any(c => !char.IsLetter(c) && !char.IsDigit(c)))
                    return BadRequest("E17: Error, password must contains at least 1 special character");
                else if (!userUdpate.NewPass.Any(c => char.IsDigit(c)))
                    return BadRequest("E18: Error, password must contains at least 1 number");
                else if (!userUdpate.NewPass.Any(c => char.IsLower(c)))
                    return BadRequest("E19: Error, password must contains at least 1 lower character");
                else if (!userUdpate.NewPass.Any(c => char.IsUpper(c)))
                    return BadRequest("E20: Error, password must contains at least 1 upper character");
                else if (userUdpate.NewPass.Length < 8)
                    return BadRequest("E21: Error, password must be at least 8 characters long");
                else if (userUdpate.NewPass.Length > 40)
                    return BadRequest("E22: Error, password must be at most 40 characters long");

                user.PasswordHash = _passwordHasher.HashPassword(user, userUdpate.NewPass);
                _context.Entry(user).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id)) return NotFound();
                    else throw;
                }

                return Ok();
            }
            else return Unauthorized();
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<int>> CreateUser(UserCreate userPayload)
        {

            var emailValidationAttribute = new EmailAddressAttribute();

            if (userPayload.Email.IsNullOrEmpty()) 
                return BadRequest("E1: Error, email is required");
            else if (!emailValidationAttribute.IsValid(userPayload.Email)) 
                return BadRequest("E2: Error, email is invalid");
            else if (await _context.User.FirstOrDefaultAsync(u => u.Data.Email == userPayload.Email) != null)
                return BadRequest("E3: Error, email already assigned to another account");

            if (userPayload.Username.IsNullOrEmpty()) 
                return BadRequest("E4: Error, username is required");
            else if (emailValidationAttribute.IsValid(userPayload.Username)) 
                return BadRequest("E5: Error, username cannot be an email");
            else if (await _context.User.FirstOrDefaultAsync(u => u.Data.Username == userPayload.Username) != null)
                return BadRequest("E6: Error, username already assigned to another account");
            else if (userPayload.Username.Length < 6) 
                return BadRequest("E7: Error, username must be at least 6 characters long");
            else if (userPayload.Username.Length > 20) 
                return BadRequest("E8: Error, username must be at most 20 characters long");

            if (userPayload.FirstName.IsNullOrEmpty())
                return BadRequest("E8: Error, firstname cannot be empty");
            else if (!userPayload.FirstName.All(c => char.IsLetter(c))) 
                return BadRequest("E9: Error, firstname can only contain letters");
            else if (userPayload.FirstName.Length < 1) 
                return BadRequest("E10: Error, firstname must be at least 1 characters long");
            else if (userPayload.FirstName.Length > 40) 
                return BadRequest("E11: Error, firstname must be at most 40 characters long");

            if (userPayload.LastName.IsNullOrEmpty())
                return BadRequest("E12: Error, lastname cannot be empty");
            else if (!userPayload.LastName.All(c => char.IsLetter(c))) 
                return BadRequest("E13: Error, lastname can only contain letters");
            else if (userPayload.LastName.Length < 1) 
                return BadRequest("E14: Error, lastname must be at least 1 characters long");
            else if (userPayload.LastName.Length > 40) 
                return BadRequest("E15: Error, lastname must be at most 40 characters long");

            if (userPayload.Password.IsNullOrEmpty())
                return BadRequest("E16: Error, password cannot be empty");
            else if (!userPayload.Password.Any(c => !char.IsLetter(c) && !char.IsDigit(c)))
                return BadRequest("E17: Error, password must contains at least 1 special character");
            else if (!userPayload.Password.Any(c => char.IsDigit(c)))
                return BadRequest("E18: Error, password must contains at least 1 number");
            else if (!userPayload.Password.Any(c => char.IsLower(c)))
                return BadRequest("E19: Error, password must contains at least 1 lower character");
            else if (!userPayload.Password.Any(c => char.IsUpper(c)))
                return BadRequest("E20: Error, password must contains at least 1 upper character");
            else if (userPayload.Password.Length < 8) 
                return BadRequest("E21: Error, password must be at least 8 characters long");
            else if (userPayload.Password.Length > 40) 
                return BadRequest("E22: Error, password must be at most 40 characters long");

            if (userPayload.BirthDate.Year < 1900)
                return BadRequest("E23: Error, birthdate must be after 1900");
            else if (userPayload.BirthDate.Year > DateTime.Now.Year - 8)
                return BadRequest($"E24: Error, birthdate must be before {DateTime.Now.Year - 8}");

            User user = new()
            {
                Data = userPayload,
                Id = 0,
                PasswordHash = "",
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, userPayload.Password);

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }


        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<UserInfo>> Login(UserLogin userLogin)
        {
            var emailValidationAttribute = new EmailAddressAttribute();
            User? user;

            if (emailValidationAttribute.IsValid(userLogin.UsernameOrEmail))
                user = await _context.User.FirstOrDefaultAsync(u => u.Data.Email == userLogin.UsernameOrEmail);
            else user = await _context.User.FirstOrDefaultAsync(u => u.Data.Username == userLogin.UsernameOrEmail);

            if (user == null) return NotFound(); // User with the given username does not exist

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLogin.Pass) == PasswordVerificationResult.Success)
                return Ok(new UserInfo { Data = user.Data, Id = user.Id }); // Passwords match, authentication successful
            else return Unauthorized(); // Passwords do not match, authentication failed
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok();
        }


        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
