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
        public List<Allergen> Allergens { get; private set; }
        public List<Diet> DietTypes { get; set; }
        public List<string> Instructions { get; set; }
        public int Portions { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public void AssignId(int id)
        {
            Id = id;
        }
        public List<Allergen> GetAllergens()
        {
            Allergens = Ingredients.SelectMany(i => i.Allergens).Distinct().ToList();
            return Allergens;
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
                    Allergens = new List<Allergen>(i.Allergens)
                }).ToList(),
                DietTypes = new List<Diet>(this.DietTypes),
                Instructions = new List<string>(this.Instructions),
                Portions = this.Portions,
                Difficulty = this.Difficulty
            };
            clonedRecipe.AssignId(this.Id);
            return clonedRecipe;
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
        Easy,//0
        Medium,//1
        Hard//2
    }
}
