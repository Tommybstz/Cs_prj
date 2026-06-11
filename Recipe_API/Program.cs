using Recipe_API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

List<Recipe_API.Recipe> recipes = new List<Recipe_API.Recipe>();

//add a new recipe
app.MapPost("/recipes", (Recipe recipe) =>
{
    var error = ValidateRecipe(recipe);

    if (error != null)
    {
        return Results.BadRequest(error);
    }

    recipe.AssignId(recipes.Count == 0 ? 1 : recipes.Max(r => r.Id) + 1);//assing unique Id

    recipes.Add(recipe);

    return Results.Created($"/recipes/{recipe.Id}", recipe);
});
app.MapGet("/recipes/{id}", (int id) =>
{
    var recipe = recipes.FirstOrDefault(r => r.Id == id);

    if (recipe == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(recipe);
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
app.MapGet("/recipes", (Diet? diet, DifficultyLevel? difficulty, Allergen? allergen,string?search) =>
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
app.Run();

static string? ValidateRecipe(Recipe recipe)
{
    if (string.IsNullOrEmpty(recipe.Name))//early return if recipe name is missing
    {
        return "Recipe name is required.";
    }
    if (recipe.Ingredients.Count == 0)//early return if no ingredients are provided
    {
        return "At least one ingredient is required.";
    }
    if (recipe.Portions <= 0)//early return if portions is not positive
    {
        return "Portions must be greater than zero.";
    }
    if (!Enum.IsDefined(typeof(DifficultyLevel), recipe.Difficulty))//early return if difficulty is not valid
    {
        return "Invalid difficulty level.";
    }
    if (recipe.DietTypes.Any(d => !Enum.IsDefined(typeof(Diet), d)))//early return if diet type is not valid
    {
        return "Invalid diet type.";
    }

    return null;//validation passed
}
