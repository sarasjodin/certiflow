using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertiFlowApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToolCalibrationValidUntilToDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalibrationValidUntilUtc",
                table: "Tools");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CalibrationValidUntil",
                table: "Tools",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalibrationValidUntil",
                table: "Tools");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CalibrationValidUntilUtc",
                table: "Tools",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
