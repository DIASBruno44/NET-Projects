using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    // DTO utilisé par l'endpoint PUT pour modifier une étape existante.
    public class InstructionStepUpdateDto
    {
        [Required(ErrorMessage = "La description de l'étape est obligatoire.")]
        public string StepDescription { get; set; }

        [Required(ErrorMessage = "L'ordre de l'étape est obligatoire.")]
        [Range(1, 100, ErrorMessage = "L'ordre doit être un nombre positif.")]
        public int Order { get; set; }
    }
}