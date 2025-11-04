using MeteoApp.Core.Models;
using System.Threading.Tasks;

namespace MeteoApp.Services
{
    public interface IMeteoServices
    {
        Task<MeteoJour> GetMeteoAsync(string nomVille);

        Task<Previsions> GetPrevisionsAsync(string nomVille);
    }
}
