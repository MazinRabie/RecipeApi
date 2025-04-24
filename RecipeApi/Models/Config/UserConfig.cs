using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecipeApi.Models.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Recipes).WithOne(x => x.author).HasForeignKey(x => x.AuthorId);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
