﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ToDo.Infrastructure;

#nullable disable

namespace ToDo.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ToDo.Domain.Entities.ChecklistItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Completed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("TaskId")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("ChecklistItem", (string)null);
                });

            modelBuilder.Entity("ToDo.Domain.Entities.Tasks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tasks", (string)null);
                });

            modelBuilder.Entity("ToDo.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1264),
                            Name = "John Doe",
                            Role = 1
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1275),
                            Name = "Jane Smith",
                            Role = 1
                        },
                        new
                        {
                            Id = 3,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1275),
                            Name = "Michael Johnson",
                            Role = 1
                        },
                        new
                        {
                            Id = 4,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1276),
                            Name = "Emily Davis",
                            Role = 1
                        },
                        new
                        {
                            Id = 5,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1277),
                            Name = "Robert Wilson",
                            Role = 1
                        },
                        new
                        {
                            Id = 6,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1277),
                            Name = "Sarah Brown",
                            Role = 1
                        },
                        new
                        {
                            Id = 7,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1278),
                            Name = "David Miller",
                            Role = 1
                        },
                        new
                        {
                            Id = 8,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1278),
                            Name = "Lisa Taylor",
                            Role = 1
                        },
                        new
                        {
                            Id = 9,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1279),
                            Name = "James Anderson",
                            Role = 1
                        },
                        new
                        {
                            Id = 10,
                            CreatedAt = new DateTime(2025, 6, 17, 12, 46, 2, 365, DateTimeKind.Unspecified).AddTicks(1279),
                            Name = "Jennifer Thomas",
                            Role = 1
                        });
                });

            modelBuilder.Entity("ToDo.Domain.Entities.ChecklistItem", b =>
                {
                    b.HasOne("ToDo.Domain.Entities.Tasks", "Task")
                        .WithMany("Checklist")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Task");
                });

            modelBuilder.Entity("ToDo.Domain.Entities.Tasks", b =>
                {
                    b.HasOne("ToDo.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ToDo.Domain.Entities.Tasks", b =>
                {
                    b.Navigation("Checklist");
                });
#pragma warning restore 612, 618
        }
    }
}
