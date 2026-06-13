using Microsoft.EntityFrameworkCore;
namespace Recipe_API
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

        public Recipe AddRecipe(Recipe recipe)
        {
            recipe.AssignId(_db.Recipes.Any() ? _db.Recipes.Max(r => r.Id) + 1 : 1);//assing unique Id

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
                _logger.LogError(ex, "Failed to add recipe ID: {RecipeId}", recipe.Id);
                throw;
            }

            return recipe;
        }
        public Recipe? GetById(int id, int? portionsRequested)
        {
            var recipe = _db.Recipes.Include(r => r.Ingredients).FirstOrDefault(r => r.Id == id)?.Clone();//gets the wanted recipe and clones it to avoid modifying the original one when adjusting ingredient quantities
            if (recipe == null) return null;

            if (portionsRequested.HasValue && portionsRequested > 0)
            {
                recipe.Ingredients.ForEach(i => i.Quantity = i.Quantity / recipe.Portions * portionsRequested.Value);
            }

            return recipe;
        }
        public List<Recipe> GetRecipes(Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? search)
        {
            var filteredRecipes = _db.Recipes.Include(r => r.Ingredients).ToList();//gets all recipes that match the requested diet type


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
                _logger.LogError(ex, "Failed to remove recipe ID: {RecipeId}", recipe.Id);
                throw;//rethrowing the exception to be handled by the calling code
            }
            return true;
        }
        public bool DeleteRecipes(Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? recipeName)
        {
            if (diet == null && difficulty == null && allergen == null && string.IsNullOrEmpty(recipeName))
            {
                _logger.LogWarning("DeleteRecipes called without any criteria. Operation aborted to prevent accidental mass deletion.");
                return false;//no criteria provided, so no recipes will be deleted
            }

            var recipesToDelete = _db.Recipes.Where(r =>
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
        public bool DeleteAllRecipes(bool? confirm)
        {
            if (confirm != true)
            {
                _logger.LogWarning("DeleteAllRecipes called without confirmation. Operation aborted.");
                return false;
            }

            _logger.LogInformation("Attempting to clear the database");
            _db.Recipes.RemoveRange(_db.Recipes.ToList());
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
        public bool UpdateRecipe(int id, Recipe updatedRecipe)
        {
            var existing = _db.Recipes.FirstOrDefault(r => r.Id == id);

            if (existing == null)
            {
                _logger.LogWarning("Attempted to update non-existent recipe with ID: {RecipeId}", id);
                return false;
            }

            _logger.LogInformation("Attempting to update recipe ID: {RecipeId}", existing.Id);
            existing.Name = updatedRecipe.Name;
            existing.Description = updatedRecipe.Description;
            existing.Instructions = updatedRecipe.Instructions;
            existing.Portions = updatedRecipe.Portions;
            existing.Difficulty = updatedRecipe.Difficulty;
            existing.DietTypes = updatedRecipe.DietTypes;
            existing.Ingredients = updatedRecipe.Ingredients;
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
