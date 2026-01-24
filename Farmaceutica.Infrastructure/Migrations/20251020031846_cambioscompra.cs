using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmaceutica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class cambioscompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleCompra_Productos_ProductoId",
                table: "DetalleCompra");

            migrationBuilder.CreateTable(
                name: "DetalleCompraListarDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Producto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unidad = table.Column<int>(type: "int", nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    TotalCompra = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TotalesCompraDto",
                columns: table => new
                {
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Igv = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleCompra_Productos_ProductoId",
                table: "DetalleCompra",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleCompra_Productos_ProductoId",
                table: "DetalleCompra");

            migrationBuilder.DropTable(
                name: "DetalleCompraListarDto");

            migrationBuilder.DropTable(
                name: "TotalesCompraDto");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleCompra_Productos_ProductoId",
                table: "DetalleCompra",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
