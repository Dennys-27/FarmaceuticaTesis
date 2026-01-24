using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class SubCategoriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Filter { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } // 👈 nombre de la categoria
        public bool IsActive { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
