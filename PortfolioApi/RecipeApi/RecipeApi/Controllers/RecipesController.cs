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
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper; // ⬅️ L'outil de conversion DTO/Entité

        // 💡 Constructeur : Injection des dépendances (Repository et Mapper)
        public RecipesController(IRecipeRepository recipeRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        [HttpGet] // Répond à GET /api/recipes
        public async Task<ActionResult<IEnumerable<RecipeSummaryDto>>> GetRecipes()
        {
            // 1. Appel au Repository (Accès à la DB)
            var recipeEntities = await _recipeRepository.GetAllRecipesAsync();

            // 2. Mapping Entité -> DTO (Conversion sécurisée)
            var recipeDtos = _mapper.Map<IEnumerable<RecipeSummaryDto>>(recipeEntities);

            // 3. Renvoi de la réponse HTTP 200 OK
            return Ok(recipeDtos);
        }

        // Dans RecipesController.cs

        [HttpGet("{id}")] // Répond à une requête GET avec un ID dans l'URL, ex: /api/recipes/5
        public async Task<ActionResult<RecipeDetailDto>> GetRecipeById(int id) // ⬅️ Changement du type de retour
        {
            // 1. Récupération : Le Repository charge déjà les ingrédients (.Include())
            var recipeEntity = await _recipeRepository.GetRecipeByIdAsync(id);

            if (recipeEntity == null)
            {
                return NotFound();
            }

            // 2. 🚨 Mapping vers le DTO Détaillé 🚨
            var recipeDto = _mapper.Map<RecipeDetailDto>(recipeEntity);

            // 3. Renvoi de la réponse 200 OK avec le DTO détaillé (avec la liste d'ingrédients)
            return Ok(recipeDto);
        }

        [HttpGet("{recipeId}/ingredients/{ingredientId}")]
        public async Task<ActionResult<IngredientDto>> GetIngredientForRecipe( int recipeId, int ingredientId)
        {
            // 1. Appel au Repository (Vérifie l'existence et l'appartenance à la recette)
            var ingredientEntity = await _recipeRepository.GetIngredientForRecipeAsync(recipeId, ingredientId);

            // 2. Vérification
            if (ingredientEntity == null)
            {
                return NotFound(); // Renvoie 404 Not Found
            }

            // 3. Mapping Entité -> DTO pour le renvoi
            var ingredientDto = _mapper.Map<IngredientDto>(ingredientEntity);

            // 4. Renvoi de la réponse 200 OK
            return Ok(ingredientDto);
        }

        [HttpPost] // Répond à POST /api/recipes
        public async Task<ActionResult<RecipeSummaryDto>> CreateRecipe([FromBody] RecipeCreateDto recipeDto)
        {
            // 💡 1. Validation automatique par ASP.NET Core
            if (!ModelState.IsValid)
            {
                // Renvoie une erreur 400 Bad Request avec les détails de la validation.
                return BadRequest(ModelState);
            }

            // 2. Mapping DTO -> Entité (Conversion pour la DB)
            var recipeEntity = _mapper.Map<Recipe>(recipeDto);

            // 3. Appel au Repository (Ajout de l'entité)
            _recipeRepository.AddRecipe(recipeEntity);

            // 4. Sauvegarde dans la base de données
            await _recipeRepository.SaveChangesAsync();

            // 5. Mapping de l'Entité Sauvegardée vers le DTO de résumé pour le retour
            var createdRecipeDto = _mapper.Map<RecipeSummaryDto>(recipeEntity);

            // 6. Renvoi de la réponse 201 Created avec l'URL de la nouvelle ressource (bonne pratique)
            return CreatedAtAction(nameof(GetRecipeById), new { id = createdRecipeDto.Id }, createdRecipeDto);
        }

        [HttpPost("{recipeId}/ingredients")]
        public async Task<ActionResult<IngredientDto>> AddIngredientToRecipe( int recipeId, IngredientCreateDto ingredientDto)
        {
            // 1. Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Récupération de la Recette mère (le Repository charge les ingrédients)
            var recipeEntity = await _recipeRepository.GetRecipeWithIngredientsAsync(recipeId);

            if (recipeEntity == null)
            {
                return NotFound(); // La recette mère n'existe pas
            }

            // 3. Mapping DTO -> Entité
            var ingredientEntity = _mapper.Map<Ingredient>(ingredientDto);

            // 4. Établir la relation (Ajout de l'ingrédient à la collection de la recette)
            recipeEntity.Ingredients.Add(ingredientEntity);

            // 5. Sauvegarde (EF Core insère le nouvel ingrédient et met à jour la clé étrangère)
            await _recipeRepository.SaveChangesAsync();

            // 6. Mapping de l'Entité Ingredient vers le DTO de sortie
            var ingredientToReturn = _mapper.Map<IngredientDto>(ingredientEntity);

            // 7. Réponse 201 Created (bonne pratique)
            return CreatedAtAction( nameof(GetRecipeById), new { id = recipeId }, ingredientToReturn);
        }

        [HttpPut("{id}")] // Répond à PUT avec l'ID dans l'URL
        public async Task<ActionResult> UpdateRecipe(int id, RecipeUpdateDto recipeDto)
        {
            // 1. Validation automatique (titre, PrepTimeMinutes)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Récupérer l'entité existante (avec ses ingrédients si nécessaire)
            var recipeEntity = await _recipeRepository.GetRecipeByIdAsync(id);

            // 3. Vérification : Si la ressource n'existe pas, renvoyer 404 Not Found
            if (recipeEntity == null)
            {
                return NotFound();
            }

            // 4. Mapping DTO -> Entité (Mise à jour des champs de l'entité par AutoMapper)
            // AutoMapper prend les valeurs du DTO et les copie sur l'entité existante
            _mapper.Map(recipeDto, recipeEntity);

            // 5. Sauvegarde des changements
            var success = await _recipeRepository.SaveChangesAsync();

            if (!success)
            {
                // Erreur 500 : Si la sauvegarde a échoué pour une raison DB
                return StatusCode(500, "Échec de la sauvegarde des changements.");
            }

            // 6. Renvoi d'une réponse 204 No Content (bonne pratique REST pour un succès sans corps)
            return NoContent();
        }

        [HttpPut("{recipeId}/ingredients/{ingredientId}")]
        public async Task<ActionResult> UpdateIngredient( int recipeId, int ingredientId, IngredientUpdateDto ingredientDto)
        {
            // 1. Validation de l'entrée
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Récupérer l'entité existante (en vérifiant qu'elle appartient à la recette)
            var ingredientEntity = await _recipeRepository.GetIngredientForRecipeAsync(recipeId, ingredientId);

            if (ingredientEntity == null)
            {
                return NotFound(); // Renvoie 404
            }

            // 3. Mapping DTO -> Entité (Copie des nouvelles valeurs sur l'entité existante)
            // 💡 EF Core détecte les changements ici grâce à sa fonctionnalité de suivi
            _mapper.Map(ingredientDto, ingredientEntity);

            // 4. Sauvegarde des changements
            var success = await _recipeRepository.SaveChangesAsync();

            if (!success)
            {
                return StatusCode(500, "Échec de la mise à jour de l'ingrédient.");
            }

            // 5. Renvoi de la réponse 204 No Content (succès sans corps)
            return NoContent();
        }

        [HttpDelete("{id}")] // Répond à DELETE avec l'ID dans l'URL
        public async Task<ActionResult> DeleteRecipe(int id)
        {
            // 1. Récupérer l'entité existante
            var recipeEntity = await _recipeRepository.GetRecipeByIdAsync(id);

            // 2. Vérification : Si la ressource n'existe pas
            if (recipeEntity == null)
            {
                return NotFound(); // Renvoie 404 Not Found
            }

            // 3. Appel au Repository pour marquer l'entité pour la suppression
            _recipeRepository.DeleteRecipe(recipeEntity);

            // 4. Sauvegarde des changements dans la base de données
            var success = await _recipeRepository.SaveChangesAsync();

            if (!success)
            {
                // Erreur 500 si la DB a échoué la suppression
                return StatusCode(500, "Échec de la suppression de la recette.");
            }

            // 5. Renvoi de la réponse 204 No Content (bonne pratique REST pour un succès de suppression)
            return NoContent();
        }

        [HttpDelete("{recipeId}/ingredients/{ingredientId}")]
        public async Task<ActionResult> DeleteIngredient(int recipeId, int ingredientId)
        {
            // 1. Récupération et vérification de la propriété
            var ingredientEntity = await _recipeRepository.GetIngredientForRecipeAsync(recipeId, ingredientId);

            // 2. Vérification : Si l'ingrédient n'existe pas OU n'appartient pas à la recette
            if (ingredientEntity == null)
            {
                return NotFound(); // Renvoie 404 Not Found
            }

            // 3. Appel au Repository pour marquer l'entité pour la suppression
            _recipeRepository.DeleteIngredient(ingredientEntity);

            // 4. Sauvegarde des changements
            var success = await _recipeRepository.SaveChangesAsync();

            if (!success)
            {
                return StatusCode(500, "Échec de la suppression de l'ingrédient.");
            }

            // 5. Renvoi de la réponse 204 No Content (succès de suppression sans corps)
            return NoContent();
        }

    }
}