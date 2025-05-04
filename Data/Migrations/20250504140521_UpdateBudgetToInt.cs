using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBudgetToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Budget",
                table: "MiniProjects",
                type: "int",
                nullable: true, // Or false, depending on your entity
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true); // Or false, depending on your database
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Budget",
                table: "MiniProjects",
                type: "decimal(18,2)",
                nullable: true, // Revert to original nullability
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}

