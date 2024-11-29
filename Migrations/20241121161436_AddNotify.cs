using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWheels_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNotify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notify_AspNetUsers_UserId",
                table: "Notify");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notify",
                table: "Notify");

            migrationBuilder.RenameTable(
                name: "Notify",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_Notify_UserId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notify");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notify",
                newName: "IX_Notify_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notify",
                table: "Notify",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notify_AspNetUsers_UserId",
                table: "Notify",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
