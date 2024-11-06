using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNurseApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeVisitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UIN = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Unique Identification Number of the Patient"),
                    DateTimeManipulation = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date and time for applying the manipulation"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeVisitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeVisitations_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NurseProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false, comment: "First name of the nurse"),
                    LastName = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false, comment: "Last name of the nurse"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false, comment: "Min years of experience for this work"),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NurseProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NurseProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false, comment: "First name of the Patient"),
                    LastName = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false, comment: "Last name of the Patient"),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HomeAddress = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Addres for home manipulation of the Patient"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false, comment: "Phone number of the Patient"),
                    EmergencyContactFullName = table.Column<string>(type: "nvarchar(99)", maxLength: 99, nullable: false, comment: "Full name of relative for Emergancy call if it's needed"),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false, comment: "Second phone number for Emergancy call if it's needed"),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalManipulations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Name of the manipulation"),
                    Duration = table.Column<int>(type: "int", maxLength: 120, nullable: false, comment: "Duration of single manipualton in minutes"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "Description if the manipulation needed to."),
                    Price = table.Column<decimal>(type: "decimal(28,6)", precision: 18, scale: 2, nullable: false, comment: "Manipulattion price depents of type of manipulation, location and etc."),
                    HomeVisitationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalManipulations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalManipulations_HomeVisitations_HomeVisitationId",
                        column: x => x.HomeVisitationId,
                        principalTable: "HomeVisitations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeVisitations_ApplicationUserId",
                table: "HomeVisitations",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalManipulations_HomeVisitationId",
                table: "MedicalManipulations",
                column: "HomeVisitationId");

            migrationBuilder.CreateIndex(
                name: "IX_NurseProfiles_UserId",
                table: "NurseProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfiles_UserId",
                table: "PatientProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalManipulations");

            migrationBuilder.DropTable(
                name: "NurseProfiles");

            migrationBuilder.DropTable(
                name: "PatientProfiles");

            migrationBuilder.DropTable(
                name: "HomeVisitations");
        }
    }
}
