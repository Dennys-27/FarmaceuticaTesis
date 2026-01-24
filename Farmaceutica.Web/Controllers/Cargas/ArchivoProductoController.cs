using Farmaceutica.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace Farmaceutica.Web.Controllers.Cargas
{
    public class ArchivoProductoController : Controller
    {
        [HttpPost]
        public IActionResult UploadExcel(IFormFile archivoExcel)
        {
            try
            {
                if (archivoExcel == null || archivoExcel.Length == 0)
                    return Json(new { ok = false, mensaje = "No se recibió el archivo." });

                // Carpeta donde se guardará
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Excel");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                // Generar nombre único usando GUID
                var extension = Path.GetExtension(archivoExcel.FileName);
                var nombreUnico = $"{Path.GetFileNameWithoutExtension(archivoExcel.FileName)}_{Guid.NewGuid()}{extension}";

                var rutaArchivo = Path.Combine(carpeta, nombreUnico);

                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    archivoExcel.CopyTo(stream);
                }

                // ✅ Retornamos el nombre único
                return Json(new
                {
                    ok = true,
                    mensaje = "Archivo cargado correctamente.",
                    nombreArchivo = nombreUnico // <- este será usado para procesar
                });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error: " + ex.Message });
            }
        }




        [HttpPost]
        public async Task<IActionResult> ProcesarExcel(string nombreArchivo, int categoriaId, int subCategoriaId, [FromServices] ProcesarExcelService service)
        {
            try
            {
                var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Excel", nombreArchivo);

                if (!System.IO.File.Exists(ruta))
                    return Json(new { ok = false, mensaje = "El archivo no existe en el servidor." });

                var total = await service.ProcesarArchivoAsync(ruta, categoriaId, subCategoriaId);

                return Json(new { ok = true, mensaje = "Archivo procesado e insertado correctamente.", total });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "Error al procesar Excel: " + ex.Message });
            }
        }

    }
}
