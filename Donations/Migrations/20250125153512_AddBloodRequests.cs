using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donations.Migrations
{
    /// <inheritdoc />
    public partial class AddBloodRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Donors_UserId",
                table: "Donors");

            migrationBuilder.DropIndex(
                name: "IX_DonationCenters_UserId",
                table: "DonationCenters");

            migrationBuilder.CreateTable(
                name: "BloodRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonationCenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodTypes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodRequests_DonationCenters_DonationCenterId",
                        column: x => x.DonationCenterId,
                        principalTable: "DonationCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donors_UserId",
                table: "Donors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonationCenters_UserId",
                table: "DonationCenters",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequests_DonationCenterId",
                table: "BloodRequests",
                column: "DonationCenterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodRequests");

            migrationBuilder.DropIndex(
                name: "IX_Donors_UserId",
                table: "Donors");

            migrationBuilder.DropIndex(
                name: "IX_DonationCenters_UserId",
                table: "DonationCenters");

            migrationBuilder.CreateIndex(
                name: "IX_Donors_UserId",
                table: "Donors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DonationCenters_UserId",
                table: "DonationCenters",
                column: "UserId");
        }
    }
}
