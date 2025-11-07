using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessClub.Models.Migrations
{
    /// <inheritdoc />
    public partial class MakeAbonnementIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Abonnementen_AbonnementId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "AbonnementId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Abonnementen_AbonnementId",
                table: "AspNetUsers",
                column: "AbonnementId",
                principalTable: "Abonnementen",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Abonnementen_AbonnementId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "AbonnementId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Abonnementen_AbonnementId",
                table: "AspNetUsers",
                column: "AbonnementId",
                principalTable: "Abonnementen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
