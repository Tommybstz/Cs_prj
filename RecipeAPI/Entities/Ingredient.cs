namespace RecipeAPI.Entities
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
    [Flags]
    public enum Allergen// based on EU regulation on allergens
    {
        
        None =      0b00000000,
        Gluten =    0b00000001,
        Dairy =     0b00000010,
        Nuts =      0b00000100,
        Shellfish = 0b00001000,
        Fish =      0b00010000,
        Soy =       0b00100000,
        Eggs =      0b01000000
    }
}
