using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub.Models.Migrations
{
    /// <inheritdoc />
    public partial class VerwijderOp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Voeg alleen de ontbrekende VerwijderdOp kolommen toe
            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "Leden",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "Abonnementen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "Inschrijvingen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "Betalingen",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerwijderd",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "Leden");

            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "Abonnementen");

            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "Inschrijvingen");

            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "Betalingen");

            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsVerwijderd",
                table: "AspNetUsers");
        }
    }
}