using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RecipeApi.Entities;

namespace RecipeApi.Data
{
    public class RecipeContext : DbContext
    {
        public RecipeContext(DbContextOptions<RecipeContext> options) : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; } = default!;
        public DbSet<Ingredient> Ingredients { get; set; } = default!;

        // La table des étapes d'instruction
        public DbSet<InstructionStep> InstructionSteps { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Configuration de la relation Ingrédient (One-to-Many)
            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Recipe) // L'ingrédient a une Recette
                .WithMany(r => r.Ingredients) // La Recette a plusieurs Ingrédients
                .HasForeignKey(i => i.RecipeId); // La clé de liaison est RecipeId

            // 2. Configuration de la relation InstructionStep (One-to-Many)
            modelBuilder.Entity<InstructionStep>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.InstructionSteps)
                .HasForeignKey(i => i.RecipeId);

            // Assurer que les étapes d'instruction sont ordonnées et uniques par recette
            modelBuilder.Entity<InstructionStep>()
                .HasIndex(i => new { i.RecipeId, i.Order }) // Crée un index sur la combinaison RecetteId + Ordre
                .IsUnique(); // 🚨 C'est crucial : chaque numéro d'étape doit être unique pour une recette donnée.
        }
    }
}