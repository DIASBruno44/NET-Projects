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

        [HttpGet("{id}")] // Répond à une requête GET avec un ID dans l'URL, ex: /api/recipes/5
        public async Task<ActionResult<RecipeSummaryDto>> GetRecipeById(int id)
        {
            // 1. Appel au Repository
            var recipeEntity = await _recipeRepository.GetRecipeByIdAsync(id);

            // 2. Vérification : Si la ressource n'existe pas, renvoyer 404 Not Found
            if (recipeEntity == null)
            {
                return NotFound();
            }

            // 3. Mapping Entité -> DTO et Renvoi de la réponse 200 OK
            var recipeDto = _mapper.Map<RecipeSummaryDto>(recipeEntity);

            return Ok(recipeDto);
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
    }
}