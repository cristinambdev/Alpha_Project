using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedStatusesProperly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET IDENTITY_INSERT Statuses ON");

            migrationBuilder.Sql("INSERT INTO Statuses (Id, StatusName) VALUES (1, 'Not Started')");
            migrationBuilder.Sql("INSERT INTO Statuses (Id, StatusName) VALUES (2, 'In Progress')");
            migrationBuilder.Sql("INSERT INTO Statuses (Id, StatusName) VALUES (3, 'Completed')");
            migrationBuilder.Sql("INSERT INTO Statuses (Id, StatusName) VALUES (4, 'On Hold')");

            migrationBuilder.Sql("SET IDENTITY_INSERT Statuses OFF");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Statuses WHERE Id IN (1, 2, 3, 4)");
        }
    }
}