using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donations.Migrations
{
    /// <inheritdoc />
    public partial class UrgencyLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Urgent",
                table: "BloodRequests");

            migrationBuilder.AddColumn<int>(
                name: "UrgencyLevel",
                table: "BloodRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrgencyLevel",
                table: "BloodRequests");

            migrationBuilder.AddColumn<bool>(
                name: "Urgent",
                table: "BloodRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
