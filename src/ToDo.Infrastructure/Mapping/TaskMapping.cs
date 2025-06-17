using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Mapping
{
    public class TaskMapping : IEntityTypeConfiguration<Tasks>
    {
        public void Configure(EntityTypeBuilder<Tasks> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("varchar(200)");

            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .HasColumnType("varchar(1000)");

            builder.Property(x => x.DueDate)
              .HasColumnType("timestamp without time zone")
                .IsRequired();

            builder.Property(x => x.Priority)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.CreatedAt)
              .HasColumnType("timestamp without time zone")
              .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("timestamp without time zone");

            builder.HasMany(x => x.Checklist)
                .WithOne(x => x.Task)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 