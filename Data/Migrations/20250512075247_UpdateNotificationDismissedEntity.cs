using Microsoft.EntityFrameworkCore.Migrations;

public partial class UpdateNotificationDismissedEntity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Add Foreign Key for UserId in DismissedNotifications
        migrationBuilder.AddForeignKey(
            name: "FK_DismissedNotifications_AspNetUsers_UserId",
            table: "DismissedNotifications", 
            column: "UserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        // Add Foreign Key for NotificationId in DismissedNotifications
        migrationBuilder.AddForeignKey(
            name: "FK_DismissedNotifications_Notifications_NotificationId",
            table: "DismissedNotifications", 
            column: "NotificationId",
            principalTable: "Notifications",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Remove Foreign Key for UserId
        migrationBuilder.DropForeignKey(
            name: "FK_DismissedNotifications_AspNetUsers_UserId",
            table: "DismissedNotifications");

        // Remove Foreign Key for NotificationId
        migrationBuilder.DropForeignKey(
            name: "FK_DismissedNotifications_Notifications_NotificationId",
            table: "DismissedNotifications");
    }
}
