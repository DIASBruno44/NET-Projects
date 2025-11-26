using AutoMapper;
using RecipeApi.DTOs;
using RecipeApi.Entities;
using RecipeApi.Interfaces;

namespace RecipeApi.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _repository;
        private readonly IMapper _mapper;

        // Injection des dépendances du Repository et du Mapper
        public RecipeService(IRecipeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<RecipeSummaryDto>> GetAllRecipesAsync()
        {
            var entities = await _repository.GetAllRecipesAsync();
            return _mapper.Map<IEnumerable<RecipeSummaryDto>>(entities);
        }

        public async Task<RecipeDetailDto?> GetRecipeByIdAsync(int id)
        {
            var entity = await _repository.GetRecipeByIdAsync(id);
            if (entity == null) return null;

            return _mapper.Map<RecipeDetailDto>(entity);
        }

        public async Task<RecipeDetailDto> CreateRecipeAsync(RecipeCreateDto recipeDto)
        {
            // 1. Mapping DTO -> Entité
            var recipeEntity = _mapper.Map<Recipe>(recipeDto);

            // 2. Appel au Repository (Ajout de l'entité)
            _repository.AddRecipe(recipeEntity);

            // 3. Sauvegarde dans la base de données
            await _repository.SaveChangesAsync();

            // 4. Mapping de l'Entité Sauvegardée vers le DTO de retour (pour inclure l'Id)
            return _mapper.Map<RecipeDetailDto>(recipeEntity);
        }

        //Mise à jour d'une recette (PUT)
        public async Task<bool> UpdateRecipeAsync(int id, RecipeUpdateDto recipeDto)
        {
            // 1. Récupérer l'entité existante
            var recipeEntity = await _repository.GetRecipeByIdAsync(id);

            if (recipeEntity == null)
            {
                return false; // La recette n'existe pas
            }

            // 2. Mapping DTO -> Entité (Mise à jour des champs de l'entité par AutoMapper)
            _mapper.Map(recipeDto, recipeEntity);

            // 3. Sauvegarde des changements et retour du succès
            return await _repository.SaveChangesAsync();
        }

        //Suppression d'une recette (DELETE)
        public async Task<bool> DeleteRecipeAsync(int id)
        {
            // 1. Récupérer l'entité
            var recipeEntity = await _repository.GetRecipeByIdAsync(id);

            if (recipeEntity == null)
            {
                return true; // Considéré comme un succès si la ressource n'existe déjà plus
            }

            // 2. Appel au Repository pour marquer l'entité pour la suppression
            _repository.DeleteRecipe(recipeEntity);

            // 3. Sauvegarde des changements
            return await _repository.SaveChangesAsync();
        }

        public async Task<IngredientDto?> GetIngredientForRecipeAsync(int recipeId, int ingredientId)
        {
            // Utilise le Repository pour obtenir l'entité spécifique (vérifie l'appartenance)
            var ingredientEntity = await _repository.GetIngredientForRecipeAsync(recipeId, ingredientId);

            if (ingredientEntity == null)
            {
                return null; // Non trouvé
            }

            // Mappe l'entité vers le DTO pour le retour
            return _mapper.Map<IngredientDto>(ingredientEntity);
        }

        // AJOUT D'UN INGRÉDIENT (CREATE)
        public async Task<IngredientDto?> AddIngredientToRecipeAsync(int recipeId, IngredientCreateDto ingredientDto)
        {
            // 1. Récupération de la Recette mère (le Repository charge les ingrédients)
            // Nous utilisons GetRecipeWithIngredientsAsync car il charge la collection nécessaire à l'ajout.
            var recipeEntity = await _repository.GetRecipeWithIngredientsAsync(recipeId);

            if (recipeEntity == null)
            {
                return null; // La recette mère n'existe pas (404)
            }

            // 2. Mapping DTO -> Entité Ingredient
            var ingredientEntity = _mapper.Map<Ingredient>(ingredientDto);

            // 3. Établir la relation (Ajout à la collection de la recette mère)
            recipeEntity.Ingredients.Add(ingredientEntity);

            // 4. Sauvegarde (EF Core insère le nouvel ingrédient)
            var success = await _repository.SaveChangesAsync();

            if (!success)
            {
                // En cas d'échec de la DB (rare pour un ajout simple)
                return null;
            }

            // 5. Mapping et retour du DTO avec l'ID généré
            return _mapper.Map<IngredientDto>(ingredientEntity);
        }

        // MISE À JOUR D'UN INGRÉDIENT (UPDATE)
        public async Task<bool> UpdateIngredientAsync(int recipeId, int ingredientId, IngredientUpdateDto ingredientDto)
        {
            // 1. Récupération de l'entité existante (vérifie l'appartenance)
            var ingredientEntity = await _repository.GetIngredientForRecipeAsync(recipeId, ingredientId);

            if (ingredientEntity == null)
            {
                return false; // Non trouvé
            }

            // 2. Mapping DTO -> Entité (Copie des nouvelles valeurs sur l'entité existante)
            _mapper.Map(ingredientDto, ingredientEntity);

            // 3. Sauvegarde des changements
            return await _repository.SaveChangesAsync();
        }

        // SUPPRESSION D'UN INGRÉDIENT (DELETE)
        public async Task<bool> DeleteIngredientAsync(int recipeId, int ingredientId)
        {
            // 1. Récupération de l'entité (vérifie l'appartenance)
            var ingredientEntity = await _repository.GetIngredientForRecipeAsync(recipeId, ingredientId);

            if (ingredientEntity == null)
            {
                return true; // Si l'ingrédient n'existe déjà plus, on considère que c'est un succès.
            }

            // 2. Appel au Repository pour marquer la suppression
            _repository.DeleteIngredient(ingredientEntity);

            // 3. Sauvegarde des changements
            return await _repository.SaveChangesAsync();
        }
    }
}