using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recipe_API
{
    public class RecipeService
    {
        private readonly AppDbContext _db;
        public RecipeService(AppDbContext db)
        {
            _db = db;
        }

        public Recipe AddRecipe(Recipe recipe)
        {
            recipe.AssignId(_db.Recipes.Any() ? _db.Recipes.Max(r => r.Id) + 1 : 1);//assing unique Id

            _db.Add(recipe);

            _db.SaveChanges();
            return recipe;
        }
        public Recipe? GetById(int id, int? portionsRequested)
        {
            var recipe = _db.Recipes.Include(r=>r.Ingredients).FirstOrDefault(r => r.Id == id)?.Clone();//gets the wanted recipe and clones it to avoid modifying the original one when adjusting ingredient quantities
            if (recipe == null) return null;

            if (portionsRequested.HasValue && portionsRequested > 0)
            {
                recipe.Ingredients.ForEach(i => i.Quantity = i.Quantity / recipe.Portions * portionsRequested.Value);
            }

            return recipe;
        }
        public List<Recipe> GetRecipes(Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? search)
        {
            var filteredRecipes = _db.Recipes.Include(r=>r.Ingredients).ToList();//gets all recipes that match the requested diet type


            //filters the recipes based on the provided query parameters, if any
            if (diet.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.DietTypes.Contains(diet.Value)).ToList();//filters recipes that match the specified diet type
            }
            if (difficulty.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.Difficulty == difficulty.Value).ToList();//filters recipes that match the specified difficulty level
            }
            if (allergen.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => !r.GetAllergens().HasFlag(allergen.Value)).ToList();//filters recipes that do not contain the specified allergen
            }
            if (!string.IsNullOrEmpty(search))
            {
                filteredRecipes = filteredRecipes.Where(r => r.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase)).ToList();//filters recipes based on the search term, ignoring case sensitivity
            }
            

            return filteredRecipes;
        }
        public bool DeleteRecipe(int id)
        {
            var recipe = _db.Recipes.FirstOrDefault(r => r.Id == id);
            if (recipe == null)
            {
                return false;
            }
            _db.Remove(recipe);
            _db.SaveChanges();
            return true;
        }
        public bool DeleteRecipes(Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? recipeName)
        {
            if (diet == null && difficulty == null && allergen == null && string.IsNullOrEmpty(recipeName))
            {
                return false;//no criteria provided, so no recipes will be deleted
            }

            var recipesToDelete = _db.Recipes.Where(r =>
            (string.IsNullOrEmpty(recipeName) || r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase)) &&
            (!diet.HasValue || r.DietTypes.Contains(diet.Value)) &&
            (!difficulty.HasValue || r.Difficulty == difficulty.Value) &&
            (!allergen.HasValue || !r.GetAllergens().HasFlag(allergen.Value))
            ).ToList();

            _db.Recipes.RemoveRange(recipesToDelete);
            _db.SaveChanges();
            return true;
        }
        public bool DeleteAllRecipes(bool? confirm)
        {
            if (confirm != true)
            {
                return false;
            }
            _db.Recipes.RemoveRange(_db.Recipes.ToList());
            _db.SaveChanges();
            return true;
        }
        public bool UpdateRecipe(int id, Recipe updatedRecipe)
        {
            var existing = _db.Recipes.FirstOrDefault(r => r.Id == id);

            if (existing == null) return false;

            existing.Name = updatedRecipe.Name;
            existing.Description = updatedRecipe.Description;
            existing.Instructions = updatedRecipe.Instructions;
            existing.Portions = updatedRecipe.Portions;
            existing.Difficulty = updatedRecipe.Difficulty;
            existing.DietTypes = updatedRecipe.DietTypes;
            existing.Ingredients = updatedRecipe.Ingredients;
            _db.SaveChanges();
            return true;
        }
    }
}
