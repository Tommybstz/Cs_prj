using Microsoft.EntityFrameworkCore;
using RecipeAPI.Entities;

namespace RecipeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        //the ingredients are stored in a separate table and linked to the recipes by the RecipeId foreign key, this is done automatically by EF Core based on the navigation property in the Recipe class
        public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>().UseSqlite("Data Source=recipes.db").Options)
        {
        }
    }

}
