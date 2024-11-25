using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNurseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UINAddedToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UIN",
                table: "HomeVisitations");

            migrationBuilder.AddColumn<string>(
                name: "UIN",
                table: "PatientProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Unique Identification Number of the Patient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UIN",
                table: "PatientProfiles");

            migrationBuilder.AddColumn<string>(
                name: "UIN",
                table: "HomeVisitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Unique Identification Number of the Patient");
        }
    }
}
