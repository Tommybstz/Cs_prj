using Microsoft.EntityFrameworkCore;

namespace Recipe_API
{
    public class AppDbContext : DbContext//underlying database context for the application, it manages the connection to the database and provides access to the entities
    {
        public DbSet<Recipe> Recipes { get; set; }//creates a table for the Recipe entity in the database
        //the ingredients are stored in a separate table and linked to the recipes by the RecipeId foreign key, this is done automatically by EF Core based on the navigation property in the Recipe class
        public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>().UseSqlite("Data Source=recipes.db").Options)//configures the database context to use SQLite and specifies the database file name
        {
        }
    }

}
