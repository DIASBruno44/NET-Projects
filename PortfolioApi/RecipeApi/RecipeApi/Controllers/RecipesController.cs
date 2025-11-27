using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RecipeApi.DTOs;
using RecipeApi.Entities;
using RecipeApi.Interfaces;

namespace RecipeApi.Controllers
{
    // Attributs ASP.NET Core
    [Route("api/[controller]")] // Définit l'URL de base : /api/recipes
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService) // ⬅️ UN SEUL ARGUMENT
        {
            _recipeService = recipeService;
        }

        // -------------------------------------------------------------------
        // 1. GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeSummaryDto>>> GetRecipes()
        {
            // Le contrôleur ne fait plus que déléguer l'appel au Service.
            return Ok(await _recipeService.GetAllRecipesAsync());
        }

        // -------------------------------------------------------------------
        // 2. GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDetailDto>> GetRecipeById(int id)
        {
            var recipeDto = await _recipeService.GetRecipeByIdAsync(id);

            // Le Contrôleur gère la réponse HTTP NotFound
            if (recipeDto == null)
            {
                return NotFound();
            }
            return Ok(recipeDto);
        }

        // -------------------------------------------------------------------
        // 3. POST (Création)
        [HttpPost]
        public async Task<ActionResult<RecipeDetailDto>> CreateRecipe([FromBody] RecipeCreateDto recipeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Délégation au Service qui gère le mapping, l'ajout et la sauvegarde
            var createdRecipeDto = await _recipeService.CreateRecipeAsync(recipeDto);

            // Renvoi de la réponse 201 Created (Contrôleur seulement)
            return CreatedAtAction(
                nameof(GetRecipeById),
                new { id = createdRecipeDto.Id },
                createdRecipeDto);
        }

        // -------------------------------------------------------------------
        // 4. PUT (Mise à Jour)
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecipe(int id, RecipeUpdateDto recipeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Le Service vérifie l'existence, mappe, et sauvegarde.
            var success = await _recipeService.UpdateRecipeAsync(id, recipeDto);

            if (!success)
            {
                // Si le service retourne false, c'est que la recette n'existe pas ou la sauvegarde a échoué.
                // Ici, nous supposons que le service renvoie false si l'entité n'est pas trouvée (404)
                return NotFound();
            }

            // Réponse 204 No Content (Contrôleur seulement)
            return NoContent();
        }

        // -------------------------------------------------------------------
        // 5. DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecipe(int id)
        {
            var success = await _recipeService.DeleteRecipeAsync(id);

            if (!success)
            {
                // Si le service retourne false, soit 404, soit erreur 500
                return StatusCode(500, "La suppression a échoué.");
            }

            // Réponse 204 No Content
            return NoContent();
        }

        // -------------------------------------------------------------------
        // 6. POST INGRÉDIENT (Ajout)
        [HttpPost("{recipeId}/ingredients")]
        public async Task<ActionResult<IngredientDto>> AddIngredientToRecipe(int recipeId, IngredientCreateDto ingredientDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Le Service gère la récupération de la recette, le mapping, l'ajout et la sauvegarde.
            var ingredientToReturn = await _recipeService.AddIngredientToRecipeAsync(recipeId, ingredientDto);

            if (ingredientToReturn == null) return NotFound(); // Si la recette mère n'existe pas

            // Réponse 201 Created
            return CreatedAtAction(
                nameof(GetRecipeById),
                new { id = recipeId }, // Utilise l'ID de la recette parente
                ingredientToReturn);
        }

        // -------------------------------------------------------------------
        // 7. GET INGRÉDIENT (Détail)
        [HttpGet("{recipeId}/ingredients/{ingredientId}")]
        public async Task<ActionResult<IngredientDto>> GetIngredientForRecipe(int recipeId, int ingredientId)
        {
            // Le Service gère la récupération de l'entité et le mapping vers le DTO.
            var ingredientDto = await _recipeService.GetIngredientForRecipeAsync(recipeId, ingredientId);

            if (ingredientDto == null) return NotFound();

            return Ok(ingredientDto);
        }

        // -------------------------------------------------------------------
        // 8. PUT INGRÉDIENT (Mise à Jour)
        [HttpPut("{recipeId}/ingredients/{ingredientId}")]
        public async Task<ActionResult> UpdateIngredient(int recipeId, int ingredientId, IngredientUpdateDto ingredientDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Le Service gère la vérification d'existence, le mapping DTO->Entité, et la sauvegarde.
            var success = await _recipeService.UpdateIngredientAsync(recipeId, ingredientId, ingredientDto);

            if (!success) return NotFound(); // Si non trouvé ou échec de sauvegarde

            return NoContent(); // Réponse 204 No Content
        }

        // -------------------------------------------------------------------
        // 9. DELETE INGRÉDIENT (Suppression)
        [HttpDelete("{recipeId}/ingredients/{ingredientId}")]
        public async Task<ActionResult> DeleteIngredient(int recipeId, int ingredientId)
        {
            // Le Service gère la récupération de l'entité et l'appel à la suppression.
            var success = await _recipeService.DeleteIngredientAsync(recipeId, ingredientId);

            if (!success) return NotFound(); // Si l'ingrédient n'existait pas (404)

            return NoContent(); // Réponse 204 No Content
        }
        // -------------------------------------------------------------------
        // 10. POST /api/recipes/{recipeId}/instructions
        [HttpPost("{recipeId}/instructions")]
        public async Task<ActionResult<InstructionStepDto>> CreateInstructionStep( int recipeId, InstructionStepCreateDto stepDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Le Service gère la vérification de la recette parente et la sauvegarde.
            var stepToReturn = await _recipeService.CreateInstructionStepAsync(recipeId, stepDto);

            if (stepToReturn == null) return NotFound(); // Si la recette mère n'existe pas

            // Réponse 201 Created (On utilise GetRecipeById car l'étape est une sous-ressource)
            return CreatedAtAction(
                nameof(GetRecipeById),
                new { id = recipeId },
                stepToReturn);
        }
        // -------------------------------------------------------------------
        // 11. PUT /api/recipes/{recipeId}/instructions/{stepId}
        [HttpPut("{recipeId}/instructions/{stepId}")]
        public async Task<ActionResult> UpdateInstructionStep( int recipeId, int stepId, InstructionStepUpdateDto stepDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Le Service gère la récupération, le mapping DTO->Entité, et la sauvegarde.
            var success = await _recipeService.UpdateInstructionStepAsync(recipeId, stepId, stepDto);

            if (!success) return NotFound(); // Non trouvé ou échec de sauvegarde

            // Réponse 204 No Content
            return NoContent();
        }
        // -------------------------------------------------------------------
        // 12. DELETE /api/recipes/{recipeId}/instructions/{stepId}
        [HttpDelete("{recipeId}/instructions/{stepId}")]
        public async Task<ActionResult> DeleteInstructionStep(int recipeId, int stepId)
        {
            // Le Service gère la récupération de l'entité et l'appel à la suppression.
            var success = await _recipeService.DeleteInstructionStepAsync(recipeId, stepId);

            if (!success)
            {
                // Si l'étape n'existait pas (404) ou si la suppression a échoué en DB
                return NotFound();
            }

            // Réponse 204 No Content
            return NoContent();
        }
    }
}