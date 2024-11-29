using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNurseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsRegistratedNursePropertyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRegistrated",
                table: "NurseProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRegistrated",
                table: "NurseProfiles");
        }
    }
}
