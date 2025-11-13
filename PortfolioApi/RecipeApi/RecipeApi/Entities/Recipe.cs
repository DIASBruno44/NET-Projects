namespace RecipeApi.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }
}