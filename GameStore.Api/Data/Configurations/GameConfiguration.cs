using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data.Configurations;
public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Game> builder)
    {
        builder.Property(game => game.Price)
               .HasPrecision(5, 2);
    }
}
