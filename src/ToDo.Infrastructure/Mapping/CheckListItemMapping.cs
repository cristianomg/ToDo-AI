using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Mapping
{
    public class CheckListItemMapping : IEntityTypeConfiguration<ChecklistItem>
    {
        public void Configure(EntityTypeBuilder<ChecklistItem> builder)
        {
            builder.ToTable("ChecklistItem");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("varchar(200)");

            builder.Property(x => x.Completed)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
              .HasColumnType("timestamp without time zone")
              .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("timestamp without time zone");

            builder.HasOne(x => x.Task)
                .WithMany(x => x.Checklist)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
