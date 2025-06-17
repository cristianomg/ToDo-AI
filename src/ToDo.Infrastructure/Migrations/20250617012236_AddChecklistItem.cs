using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ToDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecklistItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItem_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(989));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(994));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(994));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(995));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(996));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(996));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(997));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(997));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(998));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 17, 1, 22, 36, 606, DateTimeKind.Utc).AddTicks(998));

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItem_TaskId",
                table: "ChecklistItem",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistItem");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2936));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2938));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2939));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2939));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2940));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2940));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2941));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2941));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2942));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 18, 51, 7, 365, DateTimeKind.Utc).AddTicks(2942));
        }
    }
}
