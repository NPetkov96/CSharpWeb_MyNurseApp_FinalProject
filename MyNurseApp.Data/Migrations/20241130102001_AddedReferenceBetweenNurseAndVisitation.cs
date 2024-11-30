using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNurseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedReferenceBetweenNurseAndVisitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "NurseProfiles",
                newName: "IsRegistrated");

            migrationBuilder.AddColumn<Guid>(
                name: "NurseId",
                table: "HomeVisitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HomeVisitations_NurseId",
                table: "HomeVisitations",
                column: "NurseId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeVisitations_NurseProfiles_NurseId",
                table: "HomeVisitations",
                column: "NurseId",
                principalTable: "NurseProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeVisitations_NurseProfiles_NurseId",
                table: "HomeVisitations");

            migrationBuilder.DropIndex(
                name: "IX_HomeVisitations_NurseId",
                table: "HomeVisitations");

            migrationBuilder.DropColumn(
                name: "NurseId",
                table: "HomeVisitations");

            migrationBuilder.RenameColumn(
                name: "IsRegistrated",
                table: "NurseProfiles",
                newName: "IsDeleted");
        }
    }
}
