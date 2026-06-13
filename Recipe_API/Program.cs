using Microsoft.EntityFrameworkCore;
using Recipe_API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<RecipeService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

//add a new recipe
app.MapPost("/recipes", (Recipe recipe, RecipeService recipeService) =>
{
    var errors = recipe.ValidateRecipe();
    if (errors.Count > 0)
    {
        return Results.BadRequest(errors);
    }

    recipeService.AddRecipe(recipe);

    return Results.Created($"/recipes/{recipe.Id}", recipe);
});
app.MapPut("/recipes/{id}", (int id, Recipe updatedRecipe, RecipeService recipeService) =>
{

    var errors = updatedRecipe.ValidateRecipe();
    if (errors.Count > 0)
    {
        return Results.BadRequest(errors);
    }
    bool response = recipeService.UpdateRecipe(id, updatedRecipe);

    if (!response)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});
app.MapGet("/recipes/{id}", (int id, int? portionsRequested, RecipeService recipeService) =>
{
    if (portionsRequested.HasValue && portionsRequested <= 0)
        return Results.BadRequest("Portions requested must be greater than zero.");

    var recipe = recipeService.GetById(id, portionsRequested);

    if (recipe == null)
        return Results.NotFound();

    return Results.Ok(recipe);
});
app.MapGet("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? search,RecipeService recipeService) =>
{
    var recipes = recipeService.GetRecipes(diet, difficulty, allergen, search); 


    if (recipes.Count == 0)
    {
        return Results.NotFound();
    }

    return Results.Ok(recipes);
});
app.MapDelete("/recipes/{id}", (int id, RecipeService recipeService) =>
{
    bool error = recipeService.DeleteRecipe(id);
    if (!error)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});
app.MapDelete("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? recipeName,RecipeService recipeService) =>
{
    bool response = recipeService.DeleteRecipes(diet, difficulty, allergen, recipeName);
    if (! response)
    {
        return Results.BadRequest("At least one filter parameter must be provided.");
    }
    return Results.NoContent();
});
app.MapDelete("/recipes/all", (bool? confirm,RecipeService recipeService) =>
{
    bool response = recipeService.DeleteAllRecipes(confirm);

    if (!response)
    {
        return Results.BadRequest("Confirmation parameter is required to delete all recipes. Set confirm=true to proceed.");
    }

    return Results.NoContent();
});
app.Run();
