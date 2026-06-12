using System;
using System.Collections.Generic;
using System.Text;

namespace Recipe_API
{
    internal class RecipeService
    {
        List<Recipe_API.Recipe> recipes = new List<Recipe_API.Recipe>();

        public Recipe AddRecipe(Recipe recipe)
        {
            recipe.AssignId(recipes.Count == 0 ? 1 : recipes.Max(r => r.Id) + 1);//assing unique Id

            recipes.Add(recipe);

            return recipe;
        }
        public Recipe? GetById(int id, int? portionsRequested)
        {
            var recipe = recipes.FirstOrDefault(r => r.Id == id)?.Clone();//gets the wanted recipe and clones it to avoid modifying the original one when adjusting ingredient quantities
            if (recipe == null) return null;

            if (portionsRequested.HasValue && portionsRequested > 0)
            {
                recipe.Ingredients.ForEach(i => i.Quantity = i.Quantity / recipe.Portions * portionsRequested.Value);
            }

            return recipe;
        }
        public List<Recipe> GetRecipes(Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? search)
        {
            var filteredRecipes = recipes;//gets all recipes that match the requested diet type


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
                filteredRecipes = filteredRecipes.Where(r => !r.GetAllergens().Contains(allergen.Value)).ToList();//filters recipes that do not contain the specified allergen
            }
            if (!string.IsNullOrEmpty(search))
            {
                filteredRecipes = filteredRecipes.Where(r => r.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase)).ToList();//filters recipes based on the search term, ignoring case sensitivity
            }

            return filteredRecipes;
        }
        public bool DeleteRecipe(int id)
        {
            var recipe = recipes.FirstOrDefault(r => r.Id == id);
            if (recipe == null)
            {
                return false;
            }
            recipes.Remove(recipe);
            return true;
        }
        public bool DeleteRecipes(Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? recipeName)
        {
            if (diet == null && difficulty == null && allergen == null && string.IsNullOrEmpty(recipeName))
            {
                return false;//no criteria provided, so no recipes will be deleted
            }

            recipes.RemoveAll(r =>
            (string.IsNullOrEmpty(recipeName) || r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase)) &&
            (!diet.HasValue || r.DietTypes.Contains(diet.Value)) &&
            (!difficulty.HasValue || r.Difficulty == difficulty.Value) &&
            (!allergen.HasValue || !r.GetAllergens().Contains(allergen.Value))
            );
            return true;
        }
        public bool DeleteAllRecipes(bool? confirm)
        {
            if (confirm != true)
            {
                return false;
            }
            recipes.Clear();
            return true;
        }
        public bool UpdateRecipe(int id, Recipe updatedRecipe)
        {
            var recipeIndex = recipes.FindIndex(r => r.Id == id);
            if (recipeIndex == -1)
            {
                return false;
            }
            updatedRecipe.AssignId(recipes[recipeIndex].Id);

            recipes[recipeIndex] = updatedRecipe;
            return true;
        }
    }
}
