using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWheels_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amentities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amentities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromotionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarTypeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarTypeId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarTypeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarTypeDetails_CarTypes_CarTypeId",
                        column: x => x.CarTypeId,
                        principalTable: "CarTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarTypeDetails_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PromotionId = table.Column<int>(type: "int", nullable: false),
                    CreateById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotions_PromotionTypes_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "PromotionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarTypeDetails_CarTypeId",
                table: "CarTypeDetails",
                column: "CarTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarTypeDetails_CompanyId",
                table: "CarTypeDetails",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_PromotionId",
                table: "Promotions",
                column: "PromotionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amentities");

            migrationBuilder.DropTable(
                name: "CarTypeDetails");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "CarTypes");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "PromotionTypes");
        }
    }
}
