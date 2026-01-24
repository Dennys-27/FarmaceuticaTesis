using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.Entities
{
    public class Categoria : BaseEntity
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string filter { get; set; }

        public ICollection<SubCategoria> SubCategorias { get; set; } = new List<SubCategoria>();
        public ICollection<Producto> Productos { get; set; }
    }
}
