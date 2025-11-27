using AutoMapper;
using RecipeApi.DTOs;
using RecipeApi.Entities;

namespace RecipeApi.Profiles
{
    // Hérite de Profile pour configurer les mappings
    public class RecipeProfiles : Profile
    {
        public RecipeProfiles()
        {
            CreateMap<Recipe, RecipeSummaryDto>();

            CreateMap<RecipeCreateDto, Recipe>();

            CreateMap<RecipeUpdateDto, Recipe>();

            CreateMap<Ingredient, IngredientDto>();

            CreateMap<Recipe, RecipeDetailDto>();

            CreateMap<IngredientCreateDto, Ingredient>();

            CreateMap<IngredientUpdateDto, Ingredient>();

            CreateMap<InstructionStepCreateDto, InstructionStep>();

            CreateMap<InstructionStepUpdateDto, InstructionStep>();

            CreateMap<InstructionStep, InstructionStepDto>();
        }
    }
}