using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub.Models.Migrations
{
    /// <inheritdoc />
    public partial class FixSoftDeleteColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BijgewerktOp",
                table: "Lessen",
                newName: "GewijzigdOp");

            migrationBuilder.RenameColumn(
                name: "BijgewerktOp",
                table: "Inschrijvingen",
                newName: "GewijzigdOp");

            migrationBuilder.RenameColumn(
                name: "BijgewerktOp",
                table: "Abonnementen",
                newName: "GewijzigdOp");

            migrationBuilder.AddColumn<DateTime>(
                name: "EindTijd",
                table: "Lessen",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTijd",
                table: "Lessen",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsVerwijderd",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerwijderdOp",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Prijs",
                table: "Abonnementen",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "LooptijdMaanden",
                table: "Abonnementen",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EindTijd",
                table: "Lessen");

            migrationBuilder.DropColumn(
                name: "StartTijd",
                table: "Lessen");

            migrationBuilder.DropColumn(
                name: "IsVerwijderd",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VerwijderdOp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LooptijdMaanden",
                table: "Abonnementen");

            migrationBuilder.RenameColumn(
                name: "GewijzigdOp",
                table: "Lessen",
                newName: "BijgewerktOp");

            migrationBuilder.RenameColumn(
                name: "GewijzigdOp",
                table: "Inschrijvingen",
                newName: "BijgewerktOp");

            migrationBuilder.RenameColumn(
                name: "GewijzigdOp",
                table: "Abonnementen",
                newName: "BijgewerktOp");

            migrationBuilder.AlterColumn<decimal>(
                name: "Prijs",
                table: "Abonnementen",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);
        }
    }
}
