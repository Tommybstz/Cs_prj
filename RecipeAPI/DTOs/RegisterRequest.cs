using RecipeAPI.Data;

namespace RecipeAPI.DTOs
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public List<string> ValidateRegisterRequest()
        {
            var errors = new List<string>();

            if (this == null)
            {
                errors.Add("Register request cannot be null.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(this.Username))
            {
                errors.Add("Username is required.");
            }
            if (this.Username.Contains(' '))
            {
                errors.Add("Username cannot contain spaces.");
            }

            if (string.IsNullOrWhiteSpace(this.Password))
            {
                errors.Add("Password is required.");
            }

            if (this.Password.Length < 6)
            {
                errors.Add("Password must be at least 6 characters long.");
            }

            return errors;
        }
    }
}
