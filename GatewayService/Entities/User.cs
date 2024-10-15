namespace UserService.Entities
{
    public enum Gender
    {
        Man,
        Woman,
        Both,
        NoBinary,
        Patatoes,
        Other,
        Dog,
        Yes,
        No,
        Unknown,
        Unown, // Zarbi
    }

    public class UserData
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public DateOnly BirthDate { get; set; }
        public required Gender Gender { get; set; }
    }

    public class UserCreate : UserData
    {
        public required string Password { get; set; }
    }


    public class UserDTO
    {
        public required UserData Data { get; set; }
        public required string Token { get; set; }
    }

    public class UserInfo
    {
        public required UserData Data { get; set; }
        public required int Id { get; set; }
    }


    public class UserLogin
    {
        public required string UsernameOrEmail { get; set; }
        public required string Pass { get; set; }
    }

    public class UserPasswordChange
    {
        public required string OldPass { get; set; }
        public required string NewPass { get; set; }
    }
}
