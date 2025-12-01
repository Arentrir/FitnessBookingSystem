using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoPathToTrainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Trainers",
                newName: "PhotoPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoPath",
                table: "Trainers",
                newName: "PhotoUrl");
        }
    }
}
