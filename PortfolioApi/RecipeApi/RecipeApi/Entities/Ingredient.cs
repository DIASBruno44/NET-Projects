namespace RecipeApi.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RecipeId { get; set; } // Clé étrangère
        public Recipe Recipe { get; set; } // Relation
        public string Quantity { get; set; }

    }
}