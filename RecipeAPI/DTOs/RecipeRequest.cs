using RecipeAPI.Entities;

namespace RecipeAPI.DTOs
{
    public class RecipeRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Diet> DietTypes { get; set; }
        public List<string> Instructions { get; set; }
        public int Portions { get; set; }
        public DifficultyLevel Difficulty { get; set; }

        public List<string> ValidateRecipe()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(this.Name))
            {
                errors.Add("Recipe name is required.");
            }
            if (this.Ingredients.Count == 0 || this.Ingredients==null)
            {
                errors.Add("At least one ingredient is required.");
            }
            if (this.Portions <= 0)
            {
                errors.Add("Portions must be greater than zero.");
            }
            if (!Enum.IsDefined(typeof(DifficultyLevel), this.Difficulty))
            {
                errors.Add("Invalid difficulty level.");
            }
            if (this.DietTypes.Any(d => !Enum.IsDefined(typeof(Diet), d)))
            {
                errors.Add("Invalid diet type.");
            }

            foreach (var ingredient in this.Ingredients)
            {
                if (string.IsNullOrEmpty(ingredient.Name))
                    errors.Add("Ingredient name is required.");
                if (ingredient.Quantity <= 0)
                    errors.Add("Ingredient quantity must be greater than zero.");
            }

            return errors;
        }
    }
}
