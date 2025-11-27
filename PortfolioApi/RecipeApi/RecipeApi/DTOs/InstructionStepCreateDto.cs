using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    // DTO utilisé par l'endpoint POST pour ajouter une nouvelle étape.
    public class InstructionStepCreateDto
    {
        [Required(ErrorMessage = "La description de l'étape est obligatoire.")]
        public string StepDescription { get; set; }

        // 💡 CRUCIAL : Le client doit spécifier l'ordre de l'étape.
        [Required(ErrorMessage = "L'ordre de l'étape est obligatoire.")]
        [Range(1, 100, ErrorMessage = "L'ordre doit être un nombre positif.")]
        public int Order { get; set; }

    }
}