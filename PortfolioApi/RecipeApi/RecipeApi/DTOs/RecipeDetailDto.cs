namespace RecipeApi.DTOs
{
    // Utilisé pour GET /api/recipes/{id}
    public class RecipeDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Instructions { get; set; }
        public int PrepTimeMinutes { get; set; }

        // 💡 Relation : Le DTO contient une collection du DTO d'Ingrédient
        public ICollection<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
    }
}