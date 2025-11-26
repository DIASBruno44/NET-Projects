using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    // Modèle des données que nous attendons pour la mise à jour d'un ingrédient
    public class IngredientUpdateDto
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "La quantité est obligatoire.")]
        public string Quantity { get; set; }

    }
}