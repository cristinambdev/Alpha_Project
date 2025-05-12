using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class UpdatedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the UserEntityId column if it exists
            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "DismissedNotifications");

            // Add the correct foreign key for UserId reference
            migrationBuilder.AddForeignKey(
                name: "FK_DismissedNotifications_AspNetUsers_UserId",
                table: "DismissedNotifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Create an index for the UserId column
            migrationBuilder.CreateIndex(
                name: "IX_DismissedNotifications_UserId",
                table: "DismissedNotifications",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the foreign key and index if rolling back
            migrationBuilder.DropForeignKey(
                name: "FK_DismissedNotifications_AspNetUsers_UserId",
                table: "DismissedNotifications");

            migrationBuilder.DropIndex(
                name: "IX_DismissedNotifications_UserId",
                table: "DismissedNotifications");

            // Add the UserEntityId column back if rolling back
            migrationBuilder.AddColumn<string>(
                name: "UserEntityId",
                table: "DismissedNotifications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
