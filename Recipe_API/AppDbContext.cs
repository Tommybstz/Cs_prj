using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recipe_API
{
    public class AppDbContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public AppDbContext() : base(new DbContextOptionsBuilder<AppDbContext>().UseSqlite("Data Source=recipes.db").Options)
        {
        }
    }

}
