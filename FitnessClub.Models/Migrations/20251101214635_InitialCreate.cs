using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitnessClub.Models.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Abonnementen",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1" });

            migrationBuilder.DeleteData(
                table: "Betalingen",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Betalingen",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Inschrijvingen",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inschrijvingen",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Leden",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Leden",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Abonnementen",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Abonnementen",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Bedrag",
                table: "Betalingen",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Prijs",
                table: "Abonnementen",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Bedrag",
                table: "Betalingen",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Prijs",
                table: "Abonnementen",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.InsertData(
                table: "Abonnementen",
                columns: new[] { "Id", "IsVerwijderd", "LooptijdMaanden", "Naam", "Prijs", "VerwijderdOp" },
                values: new object[,]
                {
                    { 1, false, 1, "Basic", 29.99m, null },
                    { 2, false, 1, "Premium", 49.99m, null },
                    { 3, false, 1, "Pro", 79.99m, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "Lid", "LID" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Achternaam", "ConcurrencyStamp", "Email", "EmailConfirmed", "Geboortedatum", "IsVerwijderd", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "VerwijderdOp", "Voornaam" },
                values: new object[] { "1", 0, "User", "a39cd3a2-35a4-45c2-8120-35585693d854", "admin@fitness.com", true, new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, null, "ADMIN@FITNESS.COM", "ADMIN@FITNESS.COM", "AQAAAAIAAYagAAAAEKvZ5eTV8t7ohO8uA7qqjcqqC7CCAJoiJP/B3PCxAuXBar+oY+CNTyarlsm4fMoGtg==", null, false, "471a656f-cfa3-4476-910d-c9c4aa4cca14", false, "admin@fitness.com", null, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1" });

            migrationBuilder.InsertData(
                table: "Leden",
                columns: new[] { "Id", "AbonnementId", "Achternaam", "Email", "Geboortedatum", "IsVerwijderd", "Telefoon", "VerwijderdOp", "Voornaam" },
                values: new object[,]
                {
                    { 1, 1, "Janssen", "jan@email.com", new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "0123456789", null, "Jan" },
                    { 2, 2, "Pieters", "marie@email.com", new DateTime(1985, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "0987654321", null, "Marie" }
                });

            migrationBuilder.InsertData(
                table: "Betalingen",
                columns: new[] { "Id", "Bedrag", "Datum", "InschrijvingId", "IsBetaald", "IsVerwijderd", "LidId", "VerwijderdOp" },
                values: new object[,]
                {
                    { 1, 29.99m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, true, false, 1, null },
                    { 2, 49.99m, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, true, false, 2, null }
                });

            migrationBuilder.InsertData(
                table: "Inschrijvingen",
                columns: new[] { "Id", "AbonnementId", "EindDatum", "IsVerwijderd", "LidId", "StartDatum", "VerwijderdOp" },
                values: new object[,]
                {
                    { 1, 1, null, false, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 2, 2, null, false, 2, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });
        }
    }
}
