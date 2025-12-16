using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecurePostManagerApi.DTOs;
using SecurePostManagerApi.Models;
using SecurePostManagerApi.Services;

namespace SecurePostManagerApi.Controllers
{
    [Route("api/Auth")] // Route par défaut : /api/auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        // 💡 Le service UserManager est fourni par ASP.NET Identity
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Création de l'objet ApplicationUser
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                AccountCreationDate = DateTime.UtcNow // Notre champ personnalisé
            };

            // 2. Appel au service UserManager pour créer l'utilisateur dans la DB
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                // Succès : l'utilisateur est créé
                return Ok("User registered successfully.");
            }
            else
            {
                // Échec : mot de passe trop simple, email déjà utilisé, etc.
                // On renvoie les erreurs d'Identity
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }
        }

        // POST: /api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Essayer de se connecter
            var result = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                isPersistent: false, // Ne pas utiliser de cookie persistant
                lockoutOnFailure: false); // Ne pas bloquer après des échecs

            if (result.Succeeded)
            {
                // 2. Récupérer l'utilisateur pour générer le Token JWT
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                // 3. Génération du Token JWT
                var token = _tokenService.CreateToken(user!);

                return Ok(new
                {
                    Token = token,
                    Email = user!.Email
                });
            }

            // Échec de la connexion
            return Unauthorized(new { Message = "Invalid login attempt." });
        }

        // GET: /api/Auth/test-secure
        [Authorize] // ⬅️ C'est l'attribut qui sécurise la route
        [HttpGet("test-secure")]
        public IActionResult TestSecureEndpoint()
        {
            // On récupère le nom/email de l'utilisateur directement à partir du Token JWT
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            return Ok(new
            {
                Message = "Accès sécurisé réussi !",
                AuthenticatedUser = userEmail
            });
        }
    }
}