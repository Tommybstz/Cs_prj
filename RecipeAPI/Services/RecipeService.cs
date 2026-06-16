using Microsoft.EntityFrameworkCore;
using RecipeAPI.Data;
using RecipeAPI.DTOs;
using RecipeAPI.Entities;
namespace RecipeAPI.Services
{
    public class RecipeService
    {
        private readonly ILogger<RecipeService> _logger;
        private readonly AppDbContext _db;
        public RecipeService(AppDbContext db, ILogger<RecipeService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public Recipe AddRecipe(RecipeRequest req,int userId)
        {
            var recipe = new Recipe(_db.Recipes.Any() ? _db.Recipes.Max(r => r.Id) + 1 : 1,userId).CopyFromRequest(req);
            _logger.LogInformation("Attempting to add recipe: {RecipeName}", recipe.Name);
            _db.Add(recipe);
            _logger.LogInformation("Recipe {RecipeId} added to memory", recipe.Id);

            try
            {
                _db.SaveChanges();
                _logger.LogInformation("Recipe {RecipeId} added to database", recipe.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add recipe ID: {RecipeId} to database", recipe.Id);
                throw;
            }

            return recipe;
        }
        public Recipe? GetRecipeById(int id,int userId, int? portionsRequested)
        {
            var recipe = _db.Recipes.Include(r => r.Ingredients).FirstOrDefault(r => r.Id == id && r.UserId==userId)?.Clone();
            if (recipe == null) return null;

            if (portionsRequested.HasValue && portionsRequested > 0)
            {
                recipe.Ingredients.ForEach(i => i.Quantity = i.Quantity / recipe.Portions * portionsRequested.Value);
            }

            return recipe;
        }
        public List<Recipe> GetRecipes(int userId,Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? search)
        {
            var filteredRecipes = _db.Recipes.Include(r => r.Ingredients).Where(r=>r.UserId==userId).ToList();

            if (diet.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.DietTypes.Contains(diet.Value)).ToList();
            }
            if (difficulty.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.Difficulty == difficulty.Value).ToList();
            }
            if (allergen.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => !r.GetAllergens().HasFlag(allergen.Value)).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                filteredRecipes = filteredRecipes.Where(r => r.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }


            return filteredRecipes;
        }
        public bool DeleteRecipe(int id,int userId)
        {
            var recipe = _db.Recipes.FirstOrDefault(r => r.Id == id && r.UserId==userId);
            if (recipe == null)
            {
                _logger.LogWarning("Attempted to delete non-existent recipe with ID: {RecipeId}", id);
                return false;
            }
            _logger.LogInformation("Attempting to delete recipe: {RecipeName} with ID: {RecipeId}", recipe.Name, recipe.Id);
            _db.Remove(recipe);
            _logger.LogInformation("Recipe {RecipeId} removed from memory", recipe.Id);
            try
            {
                _db.SaveChanges();
                _logger.LogInformation("Recipe {RecipeId} removed from database", recipe.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove recipe ID: {RecipeId} from database", recipe.Id);
                throw;//rethrowing the exception to be handled by the calling code
            }
            return true;
        }
        public bool DeleteRecipes(int userId,Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? recipeName)
        {
            if (diet == null && difficulty == null && allergen == null && string.IsNullOrEmpty(recipeName))
            {
                _logger.LogWarning("DeleteRecipes called without any criteria. Operation aborted to prevent accidental mass deletion.");
                return false;
            }

            var recipesToDelete = _db.Recipes.Where(r => r.UserId==userId&&
            (string.IsNullOrEmpty(recipeName) || r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase)) &&
            (!diet.HasValue || r.DietTypes.Contains(diet.Value)) &&
            (!difficulty.HasValue || r.Difficulty == difficulty.Value) &&
            (!allergen.HasValue || !r.GetAllergens().HasFlag(allergen.Value))
            ).ToList();

            _logger.LogInformation("Attempting to delete {Count} recipes based on provided criteria", recipesToDelete.Count);
            _db.Recipes.RemoveRange(recipesToDelete);
            _logger.LogInformation("{Count} recipes removed from memory", recipesToDelete.Count);
            try
            {
                _db.SaveChanges();
                _logger.LogInformation("{Count} recipes removed from database", recipesToDelete.Count);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to remove {Count} recipes from database", recipesToDelete.Count);
                throw;
            }
            return true;
        }
        public bool DeleteAllRecipes(int userId,bool? confirm)
        {
            if (confirm != true)
            {
                _logger.LogWarning("DeleteAllRecipes called without confirmation. Operation aborted.");
                return false;
            }

            _logger.LogInformation("Attempting to clear the database");
            _db.Recipes.RemoveRange(_db.Recipes.Where(r=>r.UserId==userId).ToList());
            try
            {
                _db.SaveChanges();
                _logger.LogInformation("Database cleared successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear the database");
                throw;
            }
            return true;
        }
        public bool UpdateRecipe(int id,int userId, RecipeRequest req)
        {
            var existing = _db.Recipes.FirstOrDefault(r => r.Id == id && r.UserId==userId);

            if (existing == null)
            {
                _logger.LogWarning("Attempted to update non-existent recipe with ID: {RecipeId}", id);
                return false;
            }

            _logger.LogInformation("Attempting to update recipe ID: {RecipeId}", existing.Id);
            existing.CopyFromRequest(req);
            _logger.LogInformation("Recipe ID: {RecipeId} updated in memory", existing.Id);
            try
            {
                _db.SaveChanges();
                _logger.LogInformation("Recipe ID: {RecipeId} updated successfully", existing.Id);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Failed to update recipe ID: {RecipeId}", existing.Id);
                throw;
            }
            return true;
        }
    }
}
