using Recipe_API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();//dependency injection for the database context, it will be created once per request and shared across the request
builder.Services.AddScoped<RecipeService>();//dependency injection for the recipe service, it will be created once per request and shared across the request

builder.Services.AddEndpointsApiExplorer();//adds support for API documentation generation, it allows to discover the API endpoints and their parameters
builder.Services.AddSwaggerGen();//adds user-friendly documentation for the API, it generates a Swagger UI that allows to test the API endpoints and see their details

var app = builder.Build();
app.UseSwagger();//middleware to serve teh generated Swagger JSON, it will be available at /swagger/v1/swagger.json endpoint
app.UseSwaggerUI();//middleware to serve the generated Swagger UI, it will be available at /swagger endpoint

//add a new recipe
app.MapPost("/recipes", (Recipe recipe, RecipeService recipeService) =>
{
    var errors = recipe.ValidateRecipe();//returns a list of error messages, if any
    if (errors.Count > 0)//if the recipe is invalid, return a bad request response with the error messages
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
