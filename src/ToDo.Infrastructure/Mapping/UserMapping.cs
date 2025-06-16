using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;

namespace ToDo.Infrastructure.Mapping
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar(100)");

            builder.Property(x => x.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.CreatedAt)
              .HasColumnType("timestamp with time zone")
              .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("timestamp with time zone");

            // Seed Data
            builder.HasData(
                new User(1, "John Doe", Role.User),
                new User(2, "Jane Smith", Role.User),
                new User(3, "Michael Johnson", Role.User),
                new User(4, "Emily Davis", Role.User),
                new User(5, "Robert Wilson", Role.User),
                new User(6, "Sarah Brown", Role.User),
                new User(7, "David Miller", Role.User),
                new User(8, "Lisa Taylor", Role.User),
                new User(9, "James Anderson", Role.User),
                new User(10, "Jennifer Thomas", Role.User)
            );
        }
    }
} 