
namespace Recipe_API
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public Allergen Allergens { get; set; }//using bitmask to store multiple allergens in one value
    }
    [Flags]//the Flags attribute allows to use bitwise operations on the enum values
    public enum Allergen : byte// based on EU regulation on allergens
    {
        //each allergen is represented as a power of 2 in binary by using the 0b prefix, so they can be combined using bitwise OR to represent multiple allergens in one value 
        None =      0b00000000,
        Gluten =    0b00000001,//rapresented as a bitmask, each allergen is a power of 2, so they can be combined using bitwise OR to represent multiple allergens in one value
        Dairy =     0b00000010,//in binary: 00000010, in decimal: 2
        Nuts =      0b00000100,
        Shellfish = 0b00001000,
        Fish =      0b00010000,
        Soy =       0b00100000,
        Eggs =      0b01000000
    }
}
