using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessBookingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Classes");
        }
    }
}
