namespace RecipeApi.Entities
{
    public class InstructionStep
    {
        public int Id { get; set; }

        // Le texte de l'étape
        public string StepDescription { get; set; }

        // Ordre de l'étape (1, 2, 3...)
        public int Order { get; set; }

        // Lien vers la recette parente
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}