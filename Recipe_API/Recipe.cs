using System;
using System.Collections.Generic;
using System.Text;

namespace Recipe_API
{
    public class Recipe
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Diet> DietTypes { get; set; }
        public List<string> Instructions { get; set; }
        public int Portions { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public void AssignId(int id)
        {
            Id = id;
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
            var clonedRecipe = new Recipe
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
            clonedRecipe.AssignId(this.Id);
            return clonedRecipe;
        }
        public List<string> ValidateRecipe()//returns an error message if the recipe is invalid, otherwise returns null
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(this.Name))//early return if recipe name is missing
            {
                errors.Add( "Recipe name is required.");
            }
            if (this.Ingredients.Count == 0)//early return if no ingredients are provided
            {
                errors.Add("At least one ingredient is required.");
            }
            if (this.Portions <= 0)//early return if portions is not positive
            {
                errors.Add("Portions must be greater than zero.");
            }
            if (!Enum.IsDefined(typeof(DifficultyLevel), this.Difficulty))//early return if difficulty is not valid
            {
                errors.Add("Invalid difficulty level.");
            }
            if (this.DietTypes.Any(d => !Enum.IsDefined(typeof(Diet), d)))//early return if diet type is not valid
            {
                errors.Add("Invalid diet type.");
            }

            foreach (var ingredient in this.Ingredients)//ingredients validation
            {
                if (string.IsNullOrEmpty(ingredient.Name))
                    errors.Add( "Ingredient name is required.");
                if (ingredient.Quantity <= 0)
                    errors.Add("Ingredient quantity must be greater than zero.");
            }

            return errors;//returns all the errors, if any
        }

    }

    public enum Diet:int
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
    public enum DifficultyLevel: int
    {
        Easy,//0
        Medium,//1
        Hard//2
    }
}
