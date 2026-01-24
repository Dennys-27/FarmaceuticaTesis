using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmaceutica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class camposusuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ruc",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "web",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ruc",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "web",
                table: "Usuarios");
        }
    }
}
