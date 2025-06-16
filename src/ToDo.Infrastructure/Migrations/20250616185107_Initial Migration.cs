using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Name", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2936), "John Doe", 1, null },
                    { 2, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2938), "Jane Smith", 1, null },
                    { 3, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2939), "Michael Johnson", 1, null },
                    { 4, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2939), "Emily Davis", 1, null },
                    { 5, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2940), "Robert Wilson", 1, null },
                    { 6, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2940), "Sarah Brown", 1, null },
                    { 7, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2941), "David Miller", 1, null },
                    { 8, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2941), "Lisa Taylor", 1, null },
                    { 9, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2942), "James Anderson", 1, null },
                    { 10, new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2942), "Jennifer Thomas", 1, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
