using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmaceutica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TablaProductosTemporal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductoTemp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockLocal = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StockDelivery = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoTemp", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoTemp_Codigo",
                table: "ProductoTemp",
                column: "Codigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoTemp");
        }
    }
}
