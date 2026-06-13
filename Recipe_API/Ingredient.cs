using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recipe_API
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public Allergen Allergens { get; set; }//int to store allergen flags, because i use bitmasks to represent multiple allergens in one integer(gluten and dairy will be represented as 3)
    }
    [Flags]
    public enum Allergen : byte// based on EU regulation on allergens
    {
        None =      0b000000000,
        Gluten =    0b000000001,//rapresented as a bitmask, each allergen is a power of 2, so they can be combined using bitwise OR to represent multiple allergens in one value
        Dairy =     0b000000010,//in binary: 00000010, in decimal: 2
        Nuts =      0b000000100,
        Shellfish = 0b000001000,
        Fish =      0b000010000,
        Soy =       0b000100000,
        Eggs =      0b001000000
    }
}
