using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Entities
{
    public class SubCategoria : BaseEntity
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Filter { get; set; }

        // 🔹 Relación con Categoría
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public ICollection<Producto> Productos { get; set; }
    }
}
