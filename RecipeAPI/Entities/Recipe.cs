using RecipeAPI.DTOs;
namespace RecipeAPI.Entities
{
    public class Recipe
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Diet> DietTypes { get; set; }
        public List<string> Instructions { get; set; }
        public int Portions { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public Recipe(int id,int userId)
        {
            Id = id;
            UserId = userId;
        }
        public Allergen GetAllergens()
        {
            Allergen combined = Allergen.None;
            foreach (var ingredient in Ingredients)
            {
                combined |= ingredient.Allergens;// |= is += for bitwise operations, it combines the allergens of all ingredients into one value like 100 + 010 = 110 (Gluten + Dairy) 
            }
            return combined;
        }
        public Recipe Clone()
        {
            var clonedRecipe = new Recipe(this.Id,this.UserId)
            {
                Name = this.Name,
                Description = this.Description,
                Ingredients = this.Ingredients.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    Allergens = i.Allergens
                }).ToList(),
                DietTypes = new List<Diet>(this.DietTypes),
                Instructions = new List<string>(this.Instructions),
                Portions = this.Portions,
                Difficulty = this.Difficulty
            };
            return clonedRecipe;
        }
        public Recipe CopyFromRequest(RecipeRequest request)
        {
            this.Name = request.Name;
            this.Description = request.Description;
            this.Ingredients = request.Ingredients.Select(i => new Ingredient
            {
                Name = i.Name,
                Quantity = i.Quantity,
                Unit = i.Unit,
                Allergens = i.Allergens
            }).ToList();
            this.DietTypes = new List<Diet>(request.DietTypes);
            this.Instructions = new List<string>(request.Instructions);
            this.Portions = request.Portions;
            this.Difficulty = request.Difficulty;
            return this;
        }
    }

    public enum Diet
    {
        Vegetarian,
        Vegan,
        GlutenFree,
        DairyFree,
        Keto,
        Paleo,
        Halal,
        Kosher
    }
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }
}
