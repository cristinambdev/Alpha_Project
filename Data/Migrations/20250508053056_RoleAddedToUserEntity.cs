using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class RoleAddedToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            // Copy roles from AspNetUserRoles to AspNetUsers by chat GPT
            migrationBuilder.Sql(@"
                UPDATE AspNetUsers
                SET Role = (
                    SELECT TOP 1 r.Name
                    FROM AspNetRoles r
                    JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                    WHERE ur.UserId = AspNetUsers.Id
                )");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }
    }
}
