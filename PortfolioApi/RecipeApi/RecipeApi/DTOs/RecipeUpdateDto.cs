using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    // Modèle des données que nous attendons du client pour mettre à jour une recette.
    public class RecipeUpdateDto
    {
        // 🚨 Validation : Les mêmes règles que pour la création
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string Title { get; set; }

        public string? Instructions { get; set; }

        [Range(0, 1440, ErrorMessage = "Le temps de préparation doit être compris entre 0 et 1440 minutes.")]
        public int PrepTimeMinutes { get; set; }
    }
}