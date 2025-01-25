using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donations.Migrations
{
    /// <inheritdoc />
    public partial class CreateAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodRequests_DonationCenters_DonationCenterId",
                table: "BloodRequests");

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BloodRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_BloodRequests_BloodRequestId",
                        column: x => x.BloodRequestId,
                        principalTable: "BloodRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "Donors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BloodRequestId",
                table: "Appointments",
                column: "BloodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DonorId",
                table: "Appointments",
                column: "DonorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodRequests_DonationCenters_DonationCenterId",
                table: "BloodRequests",
                column: "DonationCenterId",
                principalTable: "DonationCenters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodRequests_DonationCenters_DonationCenterId",
                table: "BloodRequests");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodRequests_DonationCenters_DonationCenterId",
                table: "BloodRequests",
                column: "DonationCenterId",
                principalTable: "DonationCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
