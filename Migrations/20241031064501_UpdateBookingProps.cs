using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWheels_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OwnerConfirm",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerConfirm",
                table: "Bookings");
        }
    }
}
