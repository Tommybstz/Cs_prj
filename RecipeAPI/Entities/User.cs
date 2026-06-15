namespace RecipeAPI.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }

    }
        public enum Role
        {
            User,
            Admin
        }
}
