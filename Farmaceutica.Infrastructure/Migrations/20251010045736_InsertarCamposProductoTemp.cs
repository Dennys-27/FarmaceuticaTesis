using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmaceutica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InsertarCamposProductoTemp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "ProductoTemp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "ProductoTemp",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubCategoria",
                table: "ProductoTemp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SubCategoriaId",
                table: "ProductoTemp",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "ProductoTemp");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "ProductoTemp");

            migrationBuilder.DropColumn(
                name: "SubCategoria",
                table: "ProductoTemp");

            migrationBuilder.DropColumn(
                name: "SubCategoriaId",
                table: "ProductoTemp");
        }
    }
}
