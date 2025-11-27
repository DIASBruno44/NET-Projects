namespace RecipeApi.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PrepTimeMinutes { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public ICollection<InstructionStep> InstructionSteps { get; set; } = new List<InstructionStep>();
    }
}