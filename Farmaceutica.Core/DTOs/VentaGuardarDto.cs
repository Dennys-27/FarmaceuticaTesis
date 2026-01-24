using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class VentaGuardarDto
    {
        public int VentId { get; set; }
        public int PagId { get; set; }
        public int CliId { get; set; }
        public string CliRuc { get; set; }
        public string CliDirecc { get; set; }
        public string CliCorreo { get; set; }
        public string VentComent { get; set; }
        public int MonId { get; set; }
    }


    public class  CompraGuardarDto
    {
        public int ComprId { get; set; }
        public int PagId { get; set; }
        public int ProvId { get; set; }
        public string ProvRuc { get; set; }
        public string ProvDirecc { get; set; }
        public string ProvCorreo { get; set; }
        public string ComprComent { get; set; }
        public int MonId { get; set; }
    }
}
