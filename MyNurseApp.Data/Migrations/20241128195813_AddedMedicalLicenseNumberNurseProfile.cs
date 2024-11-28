using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNurseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMedicalLicenseNumberNurseProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MedicalLicenseNumber",
                table: "NurseProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicalLicenseNumber",
                table: "NurseProfiles");
        }
    }
}
