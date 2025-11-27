using RecipeApi.DTOs;
using RecipeApi.Entities;

namespace RecipeApi.Interfaces
{
    // L'interface définit la logique métier que l'on veut exposer.
    public interface IRecipeService
    {
        // On expose les méthodes d'accès, mais elles peuvent contenir de la logique de filtrage ou de formatage
        Task<IEnumerable<RecipeSummaryDto>> GetAllRecipesAsync();
        Task<RecipeDetailDto?> GetRecipeByIdAsync(int id);

        // Méthode pour créer une recette (logique métier)
        Task<RecipeDetailDto> CreateRecipeAsync(RecipeCreateDto recipeDto);

        // Méthode pour mettre à jour une recette
        Task<bool> UpdateRecipeAsync(int id, RecipeUpdateDto recipeDto);

        // Méthode pour supprimer
        Task<bool> DeleteRecipeAsync(int id);

        // Lecture d'un seul ingrédient
        Task<IngredientDto?> GetIngredientForRecipeAsync(int recipeId, int ingredientId);

        // Ajout d'un ingrédient
        Task<IngredientDto?> AddIngredientToRecipeAsync(int recipeId, IngredientCreateDto ingredientDto);

        // Mise à jour d'un ingrédient
        Task<bool> UpdateIngredientAsync(int recipeId, int ingredientId, IngredientUpdateDto ingredientDto);

        // Suppression d'un ingrédient
        Task<bool> DeleteIngredientAsync(int recipeId, int ingredientId);

        Task<InstructionStepDto?> CreateInstructionStepAsync(int recipeId, InstructionStepCreateDto stepDto);

        Task<bool> UpdateInstructionStepAsync(int recipeId, int stepId, InstructionStepUpdateDto stepDto);

        Task<bool> DeleteInstructionStepAsync(int recipeId, int stepId);
    }
}