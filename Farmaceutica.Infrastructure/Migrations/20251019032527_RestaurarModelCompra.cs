using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmaceutica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestaurarModelCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Usuarios_ClienteId",
                table: "Compras");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Compras",
                newName: "ProveedorId");

            migrationBuilder.RenameColumn(
                name: "CliRuc",
                table: "Compras",
                newName: "ProvRuc");

            migrationBuilder.RenameColumn(
                name: "CliDireccion",
                table: "Compras",
                newName: "ProvDireccion");

            migrationBuilder.RenameColumn(
                name: "CliCorreo",
                table: "Compras",
                newName: "ProvCorreo");

            migrationBuilder.RenameIndex(
                name: "IX_Compras_ClienteId",
                table: "Compras",
                newName: "IX_Compras_ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Usuarios_ProveedorId",
                table: "Compras",
                column: "ProveedorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Usuarios_ProveedorId",
                table: "Compras");

            migrationBuilder.RenameColumn(
                name: "ProveedorId",
                table: "Compras",
                newName: "ClienteId");

            migrationBuilder.RenameColumn(
                name: "ProvRuc",
                table: "Compras",
                newName: "CliRuc");

            migrationBuilder.RenameColumn(
                name: "ProvDireccion",
                table: "Compras",
                newName: "CliDireccion");

            migrationBuilder.RenameColumn(
                name: "ProvCorreo",
                table: "Compras",
                newName: "CliCorreo");

            migrationBuilder.RenameIndex(
                name: "IX_Compras_ProveedorId",
                table: "Compras",
                newName: "IX_Compras_ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Usuarios_ClienteId",
                table: "Compras",
                column: "ClienteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
