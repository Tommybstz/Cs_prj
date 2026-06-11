using System;
using System.Collections.Generic;
using System.Text;

namespace Recipe_API
{
    public class Ingredient
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public List<Allergen> Allergens { get; set; }
    }
    public enum Allergen// based on EU regulation on allergens
    {
        None,
        Gluten,
        Dairy,
        Nuts,
        Shellfish,
        Soy,
        Eggs
    }
}
