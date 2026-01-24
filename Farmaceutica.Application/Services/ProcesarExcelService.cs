using ClosedXML.Excel;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Interfaces;
using Farmaceutica.Core.Temporales;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Farmaceutica.Application.Services
{
    public class ProcesarExcelService
    {
        private readonly IProductoTempRepository _repo;

        public ProcesarExcelService(IProductoTempRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> ProcesarArchivoAsync(string rutaArchivo, int categoriaId, int subCategoriaId)
        {
            var lista = new List<ProductoTemp>();

            using (var workbook = new XLWorkbook(rutaArchivo))
            {
                var worksheet = workbook.Worksheet(1);
                var range = worksheet.RangeUsed();
                if (range == null)
                    return 0; // Excel vacío

                var rows = range.RowsUsed().Skip(1); // Saltamos encabezado

                foreach (var row in rows)
                {
                    if (row.Cell(1).IsEmpty()) continue;

                    decimal.TryParse(row.Cell(3).GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio);
                    int.TryParse(row.Cell(4).GetString(), out int stockLocal);
                    int.TryParse(row.Cell(5).GetString(), out int stockDelivery);

                    lista.Add(new ProductoTemp
                    {
                        Nombre = row.Cell(1).GetString().Trim(),
                        Descripcion = row.Cell(2).GetString().Trim(),
                        Precio = precio,
                        StockLocal = stockLocal,
                        StockDelivery = stockDelivery,
                        Codigo = row.Cell(6).GetString().Trim(),
                        CategoriaId = categoriaId,
                        SubCategoriaId = subCategoriaId,
                        IsActive = true,
                        FechaCreacion = DateTime.Now
                    });
                }
            }

            // Limpiar con SQL directo
            await _repo.LimpiarAsync();

            // Insertar nuevo rango
            if (lista.Any())
                await _repo.InsertarRangoAsync(lista);

            return lista.Count;
        }

    }
}
