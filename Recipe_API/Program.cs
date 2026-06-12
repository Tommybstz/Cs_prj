using Recipe_API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

List<Recipe_API.Recipe> recipes = new List<Recipe_API.Recipe>();

//add a new recipe
app.MapPost("/recipes", (Recipe recipe) =>
{
    var errors = recipe.ValidateRecipe();
    if (errors.Count > 0)
    {
        return Results.BadRequest(errors);
    }

    recipe.AssignId(recipes.Count == 0 ? 1 : recipes.Max(r => r.Id) + 1);//assing unique Id

    recipes.Add(recipe);

    return Results.Created($"/recipes/{recipe.Id}", recipe);
});
app.MapPut("/recipes/{id}", (int id, Recipe updatedRecipe) =>
{

    var errors = updatedRecipe.ValidateRecipe();
    if (errors.Count > 0)
    {
        return Results.BadRequest(errors);
    }
    var recipeIndex = recipes.FindIndex(r => r.Id == id);
    if (recipeIndex == -1)
    {
        return Results.NotFound();
    }
    updatedRecipe.AssignId(recipes[recipeIndex].Id);

    recipes[recipeIndex] = updatedRecipe;
    return Results.NoContent();
});
app.MapGet("/recipes/{id}", (int id, int? portionsRequested) =>
{
    var recipe = recipes.FirstOrDefault(r => r.Id == id)?.Clone();//gets the wanted recipe and clones it to avoid modifying the original one when adjusting ingredient quantities

    if (recipe == null)
    {
        return Results.NotFound();
    }

    if (portionsRequested.HasValue && portionsRequested <= 0)
    {
        return Results.BadRequest("Portions requested must be greater than zero.");
    }

    if (portionsRequested.HasValue && portionsRequested.Value > 0)
    {
        recipe.Ingredients.ForEach(i => i.Quantity = i.Quantity / recipe.Portions * portionsRequested.Value);
    }

    return Results.Ok(recipe);
});
app.MapGet("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? search) =>
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

    if (filteredRecipes.Count == 0)
    {
        return Results.NotFound();
    }

    return Results.Ok(filteredRecipes);
});
app.MapDelete("/recipes/{id}", (int id) =>
{
    if (!recipes.Any(r => r.Id == id))
    {
        return Results.NotFound();
    }
    recipes.Remove(recipes.FirstOrDefault(r => r.Id == id));
    return Results.NoContent();
});
app.MapDelete("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen, string? recipeName) =>
{
    if(diet== null && difficulty == null && allergen == null && string.IsNullOrEmpty(recipeName))
    {
        return Results.BadRequest("At least one filter parameter must be provided.");
    }

    recipes.RemoveAll(r =>
    (string.IsNullOrEmpty(recipeName) || r.Name.StartsWith(recipeName, StringComparison.OrdinalIgnoreCase)) &&
    (!diet.HasValue || r.DietTypes.Contains(diet.Value)) &&
    (!difficulty.HasValue || r.Difficulty == difficulty.Value) &&
    (!allergen.HasValue || !r.GetAllergens().Contains(allergen.Value))
    );
    
    return Results.NoContent();
});
app.MapDelete("/recipes/all", (bool?confirm) =>
{
    if(confirm != true)
{
    return Results.BadRequest("Confirmation parameter is required to delete all recipes. Set confirm=true to proceed.");
}

    recipes.Clear();
    return Results.NoContent();
});
app.Run();
