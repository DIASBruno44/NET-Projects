using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    public class IngredientCreateDto
    {
        [Required(ErrorMessage = "Le nom de l'ingrédient est obligatoire.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "La quantité est obligatoire.")]
        public string Quantity { get; set; }

    }
}