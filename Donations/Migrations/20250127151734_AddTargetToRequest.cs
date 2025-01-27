using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donations.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DesiredMilliliters",
                table: "BloodSupplies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetMilliliters",
                table: "BloodRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredMilliliters",
                table: "BloodSupplies");

            migrationBuilder.DropColumn(
                name: "TargetMilliliters",
                table: "BloodRequests");
        }
    }
}
