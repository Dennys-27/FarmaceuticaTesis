using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmaceutica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Prueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientesDto",
                columns: table => new
                {
                    cli_id = table.Column<int>(type: "int", nullable: false),
                    cli_nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cli_ruc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cli_direcc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cli_correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cli_telf = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientesDto");
        }
    }
}
