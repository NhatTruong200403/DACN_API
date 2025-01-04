using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWheels_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Promotions_PromotionId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_PromotionId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PromotionId",
                table: "Posts",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Promotions_PromotionId",
                table: "Posts",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id");
        }
    }
}
