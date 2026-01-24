using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmaceutica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_proceso_venta_campos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "FechaVenta",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "StockAplicado",
                table: "DetalleVentas");

            migrationBuilder.RenameColumn(
                name: "TipoVenta",
                table: "Ventas",
                newName: "CliRuc");

            migrationBuilder.RenameColumn(
                name: "Observaciones",
                table: "Ventas",
                newName: "CliDireccion");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Ventas",
                newName: "CliCorreo");

            migrationBuilder.RenameColumn(
                name: "Subtotal",
                table: "DetalleVentas",
                newName: "TotalVenta");

            migrationBuilder.RenameColumn(
                name: "PrecioUnitario",
                table: "DetalleVentas",
                newName: "Precio");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Ventas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MetodoPago",
                table: "Ventas",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DocumentoTipo",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EncargadoId",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Igv",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Moneda",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Unidad",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClienteId",
                table: "Ventas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_EncargadoId",
                table: "Ventas",
                column: "EncargadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Usuarios_ClienteId",
                table: "Ventas",
                column: "ClienteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Usuarios_EncargadoId",
                table: "Ventas",
                column: "EncargadoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Usuarios_ClienteId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Usuarios_EncargadoId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_ClienteId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_EncargadoId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "DocumentoTipo",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "EncargadoId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Igv",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Moneda",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Unidad",
                table: "Ventas");

            migrationBuilder.RenameColumn(
                name: "CliRuc",
                table: "Ventas",
                newName: "TipoVenta");

            migrationBuilder.RenameColumn(
                name: "CliDireccion",
                table: "Ventas",
                newName: "Observaciones");

            migrationBuilder.RenameColumn(
                name: "CliCorreo",
                table: "Ventas",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "TotalVenta",
                table: "DetalleVentas",
                newName: "Subtotal");

            migrationBuilder.RenameColumn(
                name: "Precio",
                table: "DetalleVentas",
                newName: "PrecioUnitario");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Ventas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPago",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVenta",
                table: "Ventas",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "StockAplicado",
                table: "DetalleVentas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
