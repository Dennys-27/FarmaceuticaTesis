using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmaceutica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CamposParaManejarDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_UsuarioId",
                table: "Ventas");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Ventas",
                newName: "TipoVenta");

            migrationBuilder.AddColumn<string>(
                name: "CodigoSeguimiento",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentarioRepartidor",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionEntrega",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstadoDelivery",
                table: "Ventas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAsignacion",
                table: "Ventas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega",
                table: "Ventas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RepartidorId",
                table: "Ventas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_RepartidorId",
                table: "Ventas",
                column: "RepartidorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Usuarios_RepartidorId",
                table: "Ventas",
                column: "RepartidorId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Usuarios_RepartidorId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_RepartidorId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "CodigoSeguimiento",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "ComentarioRepartidor",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "DireccionEntrega",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "EstadoDelivery",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "FechaAsignacion",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "FechaEntrega",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "RepartidorId",
                table: "Ventas");

            migrationBuilder.RenameColumn(
                name: "TipoVenta",
                table: "Ventas",
                newName: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UsuarioId",
                table: "Ventas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Usuarios_UsuarioId",
                table: "Ventas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
