using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecurePostManagerApi.Data;
using SecurePostManagerApi.Models;
using SecurePostManagerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Récupération de la chaîne de connexion présent dans le json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Enregistrement du DbContext avec SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuration du service Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Configuration de l'Authentification JWT
builder.Services.AddAuthentication(options =>
{
    // Définir le schéma par défaut pour la validation de l'identité
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Définir le schéma par défaut pour les réponses de défi (Challenge, ex: renvoie 401)
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // Ajout du schéma JWT Bearer et configuration de la validation
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Valider la Clé de Signature (ESSENTIEL)
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),

            // Valider l'Émetteur (Issuer)
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

            // Valider l'Audience (Client)
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],

            // Valider l'Expiration
            ValidateLifetime = true,

            // Évite le délai par défaut qui peut poser problème
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Configure la politique par défaut pour utiliser le schéma JwtBearer
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "API is running! Try /api/Auth/register");
app.MapControllers();

app.Run();
