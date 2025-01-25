using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donations.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestUrgency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Urgent",
                table: "BloodRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Urgent",
                table: "BloodRequests");
        }
    }
}
