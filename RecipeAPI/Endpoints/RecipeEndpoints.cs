using Microsoft.AspNetCore.Authorization;
using RecipeAPI.DTOs;
using RecipeAPI.Entities;
using RecipeAPI.Services;
using System.Security.Claims;

namespace RecipeAPI.Endpoints
{
    public static class RecipeEndpoints
    {
        public static void MapEndPoints(this WebApplication app)
        {
            app.MapPost("/recipes", (RecipeRequest req, RecipeService recipeService, HttpContext ctx) =>
            {
                var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Results.Unauthorized();
                var userId = int.Parse(userIdClaim.Value);
                
                var errors = req.ValidateRecipe();
                if (errors.Count > 0)
                {
                    return Results.BadRequest(errors);
                }

                var recipe = recipeService.AddRecipe(req,userId);

                return Results.Created($"/recipes/{recipe.Id}", recipe);
            }).RequireAuthorization();
            app.MapPut("/recipes/{id}", (int id, RecipeRequest req, RecipeService recipeService,HttpContext ctx) =>
            {
                var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Results.Unauthorized();
                var userId = int.Parse(userIdClaim.Value);

                var errors = req.ValidateRecipe();
                if (errors.Count > 0)
                {
                    return Results.BadRequest(errors);
                }
                bool response = recipeService.UpdateRecipe(id,userId, req);

                if (!response)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            }).RequireAuthorization();
            app.MapGet("/recipes/{id}", (int id, int? portionsRequested, RecipeService recipeService,HttpContext ctx) =>
            {
                var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Results.Unauthorized();
                var userId = int.Parse(userIdClaim.Value);
                
                if (portionsRequested.HasValue && portionsRequested <= 0)
                    return Results.BadRequest("Portions requested must be greater than zero.");

                var recipe = recipeService.GetById(id,userId, portionsRequested);

                if (recipe == null)
                    return Results.NotFound();

                return Results.Ok(recipe);
            }).RequireAuthorization();
            app.MapGet("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? search, RecipeService recipeService,HttpContext ctx) =>
            {
                var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Results.Unauthorized();
                var userId = int.Parse(userIdClaim.Value);

                var recipes = recipeService.GetRecipes(userId,diet, difficulty, allergen, search);


                if (recipes.Count == 0)
                {
                    return Results.NotFound();
                }

                return Results.Ok(recipes);
            }).RequireAuthorization();
            app.MapDelete("/recipes/{id}", (int id, RecipeService recipeService,HttpContext ctx) =>
            {
                var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Results.Unauthorized();
                var userId = int.Parse(userIdClaim.Value);
                
                bool error = recipeService.DeleteRecipe(id,userId);
                if (!error)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            });
            app.MapDelete("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? recipeName, RecipeService recipeService,HttpContext ctx) =>
            {
                var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Results.Unauthorized();
                var userId = int.Parse(userIdClaim.Value);
                
                bool response = recipeService.DeleteRecipes(userId,diet, difficulty, allergen, recipeName);
                if (!response)
                {
                    return Results.BadRequest("At least one filter parameter must be provided.");
                }
                return Results.NoContent();
            }).RequireAuthorization();
            app.MapDelete("/recipes/all", (bool? confirm, RecipeService recipeService,HttpContext ctx) =>
            {
                var userIdClaim = ctx.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Results.Unauthorized();
                var userId = int.Parse(userIdClaim.Value);
                
                bool response = recipeService.DeleteAllRecipes(userId,confirm);

                if (!response)
                {
                    return Results.BadRequest("Confirmation parameter is required to delete all recipes. Set confirm=true to proceed.");
                }

                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
