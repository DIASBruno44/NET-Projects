using Microsoft.EntityFrameworkCore;
using RecipeApi.Data;
using RecipeApi.Entities;
using RecipeApi.Interfaces;

namespace RecipeApi.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeContext _context;

        // Injection de Dépendances du DbContext
        public RecipeRepository(RecipeContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            // Récupère toutes les recettes
            return await _context.Recipes.ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            // Récupère la recette ET charge les ingrédients associés
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public void AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
        }

        public async Task<bool> SaveChangesAsync()
        {
            // Retourne true si au moins une modification a été appliquée
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeleteRecipe(Recipe recipe)
        {
            // EF Core marque l'entité comme "Deleted" dans le change tracker
            _context.Recipes.Remove(recipe);
        }

        public async Task<Recipe?> GetRecipeWithIngredientsAsync(int recipeId)
        {
            // Il faut charger les ingrédients pour manipuler la liste
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == recipeId);
        }

        public async Task<Ingredient?> GetIngredientForRecipeAsync(int recipeId, int ingredientId)
        {
            // EF Core s'assure que l'ID de l'ingrédient et l'ID de la recette correspondent
            return await _context.Ingredients
                .FirstOrDefaultAsync(i => i.RecipeId == recipeId && i.Id == ingredientId);
        }

        public void DeleteIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Remove(ingredient);
        }
    }
}