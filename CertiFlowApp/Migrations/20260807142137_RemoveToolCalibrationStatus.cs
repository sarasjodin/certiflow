using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertiFlowApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveToolCalibrationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalibrationStatus",
                table: "Tools");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalibrationStatus",
                table: "Tools",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
