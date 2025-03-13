namespace eStore.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}