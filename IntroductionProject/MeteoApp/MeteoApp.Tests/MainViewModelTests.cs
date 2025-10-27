using MeteoApp.Core.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace MeteoApp.Tests
{
    public class MainViewModelTests
    {
        // On utilise la bibliothèque Moq pour créer un faux service (Mock)
        private readonly Mock<IMeteoServices> _mockMeteoService;

        // Constructeur de la classe de test (pour initialiser les Mocks)
        public MainViewModelTests()
        {
            _mockMeteoService = new Mock<IMeteoServices>();
        }

        [Fact]
        public async Task RechercherMeteoCommand_ShouldSetDataOnSuccess()
        {
            // ARRANGE (Préparation du test)

            // 1. Définir le résultat que le Mock doit retourner (une fausse donnée météo)
            var expectedMeteo = new MeteoJour
            {
                NomVille = "TestVille",
                // On peut laisser les autres propriétés nulles pour ce test simple
                DonnéesPrincipales = new MainData { TemperatureActuelle = 25.0 }
            };

            // 2. Configurer le Mock : Quand GetMeteoAsync est appelé avec n'importe quelle chaîne, retourne la fausse donnée
            _mockMeteoService.Setup(s => s.GetMeteoAsync(It.IsAny<string>()))
                             .ReturnsAsync(expectedMeteo);

            // 3. Créer le ViewModel en lui injectant le Mock (grâce à la DI !)
            var viewModel = new MainViewModel(_mockMeteoService.Object);


            // ACT (Exécution de la commande)

            // Exécuter la commande (Note: nous n'avons pas besoin de cliquer sur un bouton WPF!)
            viewModel.RechercherMeteoCommand.Execute(null);

            // Attendre un court instant pour que le Task.Run asynchrone se termine
            await Task.Delay(100);


            // ASSERT (Vérification du résultat)

            // 1. Vérifier que la propriété a bien été mise à jour
            Assert.NotNull(viewModel.MeteoDuJour);
            Assert.Equal("TestVille", viewModel.MeteoDuJour.NomVille);

            // 2. Vérifier que l'état IsBusy est revenu à false
            Assert.False(viewModel.IsBusy);
        }
    }
}