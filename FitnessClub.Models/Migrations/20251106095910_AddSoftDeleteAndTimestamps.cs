using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteAndTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "Lessen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "Inschrijvingen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "Abonnementen",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "Lessen");

            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "Inschrijvingen");

            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "Abonnementen");
        }
    }
}
