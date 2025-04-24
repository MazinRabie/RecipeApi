using Microsoft.EntityFrameworkCore;
using RecipeApi.Models;
using System.Text.Json;

namespace RecipeApi.DataStore
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).ValueGeneratedOnAdd();

                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired();
            });

            modelBuilder.Entity<User>()
       .HasMany(u => u.Favorites)
       .WithMany(r => r.FavoritedBy)
       .UsingEntity<Dictionary<string, object>>(
           "UserFavorite",
           j => j
               .HasOne<Recipe>()
               .WithMany()
               .HasForeignKey("RecipeId")
               .OnDelete(DeleteBehavior.Restrict), // or .NoAction
           j => j
               .HasOne<User>()
               .WithMany()
               .HasForeignKey("UserId")
               .OnDelete(DeleteBehavior.Restrict)  // or .NoAction
       );

            // Optional: ensure AuthorId links Recipe to User with proper delete behavior
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Cascade); // Only one cascade path, this is okay


            // Recipe
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).ValueGeneratedOnAdd();
                entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Description).HasMaxLength(1000);
                entity.HasOne(r => r.author)
                      .WithMany(u => u.Recipes)
                      .HasForeignKey(r => r.AuthorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Recipe>()
    .Property(r => r.Ingredients)
    .HasConversion(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
    );

            modelBuilder.Entity<Recipe>()
                .Property(r => r.CookingSteps)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
                );


            // Comment
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.Title).HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Recipe)
                      .WithMany(r => r.Comments)
                      .HasForeignKey(c => c.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // To keep things simple, skipping Favorites for now
            // (can be added later as many-to-many if needed)
        }
    }
}
