using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWheels_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBooking_Promotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromotionContent",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PromotionId",
                table: "Bookings",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Promotions_PromotionId",
                table: "Bookings",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Promotions_PromotionId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PromotionId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "PromotionContent",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
