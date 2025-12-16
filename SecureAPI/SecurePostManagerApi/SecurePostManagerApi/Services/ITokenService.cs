using SecurePostManagerApi.Models;

namespace SecurePostManagerApi.Services
{
    public interface ITokenService
    {
        // La méthode prend l'utilisateur et retourne le Token JWT sous forme de chaîne.
        string CreateToken(ApplicationUser user);
    }
}