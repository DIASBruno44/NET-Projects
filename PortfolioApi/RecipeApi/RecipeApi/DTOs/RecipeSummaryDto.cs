namespace RecipeApi.DTOs
{
    // Utilisé pour la liste des recettes (moins de données)
    public class RecipeSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PrepTimeMinutes { get; set; }
    }
}