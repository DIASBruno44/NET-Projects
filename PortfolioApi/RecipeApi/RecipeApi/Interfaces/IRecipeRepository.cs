using RecipeApi.Entities;

namespace RecipeApi.Interfaces
{
    public interface IRecipeRepository
    {
        // Récupérer toutes les recettes
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();

        // Récupérer une recette spécifique (avec ses ingrédients)
        Task<Recipe?> GetRecipeByIdAsync(int id);

        // Ajouter une recette
        void AddRecipe(Recipe recipe);

        // Sauvegarder les changements dans la DB
        Task<bool> SaveChangesAsync();

        void DeleteRecipe(Recipe recipe);

        Task<Recipe?> GetRecipeWithIngredientsAsync(int recipeId);

        Task<Ingredient?> GetIngredientForRecipeAsync(int recipeId, int ingredientId);

        void DeleteIngredient(Ingredient ingredient);

        Task<InstructionStep?> GetInstructionStepForRecipeAsync(int recipeId, int stepId);
        void DeleteInstructionStep(InstructionStep step);
    }
}