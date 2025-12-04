using Xunit;
using Moq;
using System.Threading.Tasks;
using RecipeApi.Interfaces;
using RecipeApi.Services;
using RecipeApi.Entities;
using RecipeApi.DTOs;
using AutoMapper;
using System.Collections.Generic;

namespace RecipeApi.Tests
{
    public class RecipeServiceTests
    {
        // Les Mocks sont les fausses versions de nos dépendances
        private readonly Mock<IRecipeRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly RecipeService _service;

        // Constructeur qui initialise les Mocks avant chaque test
        public RecipeServiceTests()
        {
            _mockRepo = new Mock<IRecipeRepository>();
            _mockMapper = new Mock<IMapper>();

            // Injection des Mocks dans le service que nous voulons tester
            _service = new RecipeService(_mockRepo.Object, _mockMapper.Object);
        }

        // ---------------------------------------------------------------------------------
        // TEST 1 : Récupération d'une recette existante
        // ---------------------------------------------------------------------------------
        [Fact]
        public async Task GetRecipeByIdAsync_ShouldReturnMappedDto_WhenRecipeExists()
        {
            // ARRANGE (Préparation)
            int recipeId = 1;

            // 1. Définir les Entités et DTOs de travail
            var mockEntity = new Recipe { Id = recipeId, Title = "Soupe" };
            var expectedDto = new RecipeDetailDto { Id = recipeId, Title = "Soupe" };

            // 2. 💡 Script du Repository (Moq) : Quand GetRecipeByIdAsync est appelé, retourne l'Entité.

            _mockRepo.Setup(r => r.GetRecipeByIdAsync(recipeId))
                     .ReturnsAsync(mockEntity);

            // 3. Script du Mapper (Moq) : Quand l'Entité est mappée, retourne le DTO attendu.
            _mockMapper.Setup(m => m.Map<RecipeDetailDto>(mockEntity))
                       .Returns(expectedDto);


            // ACT (Exécution)
            var result = await _service.GetRecipeByIdAsync(recipeId);


            // ASSERT (Vérification)
            Assert.NotNull(result);
            Assert.Equal(recipeId, result.Id);
            Assert.IsType<RecipeDetailDto>(result);
        }

        // ---------------------------------------------------------------------------------
        // TEST 2 : Récupération d'une recette inexistante
        // ---------------------------------------------------------------------------------
        [Fact]
        public async Task GetRecipeByIdAsync_ShouldReturnNull_WhenRecipeDoesNotExist()
        {
            // ARRANGE (Préparation)
            int nonexistentId = 99;

            // 1. Script du Repository (Moq) : Quand GetRecipeByIdAsync est appelé, retourne null.
            _mockRepo.Setup(r => r.GetRecipeByIdAsync(nonexistentId))
                     .ReturnsAsync((Recipe?)null); // Spécifie qu'il retourne null de manière asynchrone

            // ACT (Exécution)
            var result = await _service.GetRecipeByIdAsync(nonexistentId);


            // ASSERT (Vérification)
            Assert.Null(result);
        }
    }
}