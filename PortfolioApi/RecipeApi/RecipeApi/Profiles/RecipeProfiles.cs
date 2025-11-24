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
        }
    }
}