using Microsoft.AspNetCore.Authorization;
using RecipeAPI.DTOs;
using RecipeAPI.Entities;
using RecipeAPI.Services;

namespace RecipeAPI.Endpoints
{
    public static class RecipeEndpoints
    {
        public static void MapEndPoints(this WebApplication app)
        {
            app.MapPost("/recipes", (RecipeRequest req, RecipeService recipeService) =>
            {
                var errors = req.ValidateRecipe();
                if (errors.Count > 0)
                {
                    return Results.BadRequest(errors);
                }

                var recipe = recipeService.AddRecipe(req);

                return Results.Created($"/recipes/{recipe.Id}", recipe);
            }).RequireAuthorization();
            app.MapPut("/recipes/{id}", (int id, RecipeRequest req, RecipeService recipeService) =>
            {

                var errors = req.ValidateRecipe();
                if (errors.Count > 0)
                {
                    return Results.BadRequest(errors);
                }
                bool response = recipeService.UpdateRecipe(id, req);

                if (!response)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            }).RequireAuthorization();
            app.MapGet("/recipes/{id}", (int id, int? portionsRequested, RecipeService recipeService) =>
            {
                if (portionsRequested.HasValue && portionsRequested <= 0)
                    return Results.BadRequest("Portions requested must be greater than zero.");

                var recipe = recipeService.GetById(id, portionsRequested);

                if (recipe == null)
                    return Results.NotFound();

                return Results.Ok(recipe);
            }).RequireAuthorization();
            app.MapGet("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? search, RecipeService recipeService) =>
            {
                var recipes = recipeService.GetRecipes(diet, difficulty, allergen, search);


                /*if (recipes.Count == 0)
                {
                    return Results.NotFound();
                }*/

                return Results.Ok(recipes);
            }).RequireAuthorization();
            app.MapDelete("/recipes/{id}", (int id, RecipeService recipeService) =>
            {
                bool error = recipeService.DeleteRecipe(id);
                if (!error)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            });
            app.MapDelete("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? recipeName, RecipeService recipeService) =>
            {
                bool response = recipeService.DeleteRecipes(diet, difficulty, allergen, recipeName);
                if (!response)
                {
                    return Results.BadRequest("At least one filter parameter must be provided.");
                }
                return Results.NoContent();
            }).RequireAuthorization();
            app.MapDelete("/recipes/all", (bool? confirm, RecipeService recipeService) =>
            {
                bool response = recipeService.DeleteAllRecipes(confirm);

                if (!response)
                {
                    return Results.BadRequest("Confirmation parameter is required to delete all recipes. Set confirm=true to proceed.");
                }

                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
