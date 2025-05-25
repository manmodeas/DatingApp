using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntityModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Introductions",
                table: "Users",
                newName: "Introduction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Introduction",
                table: "Users",
                newName: "Introductions");
        }
    }
}
