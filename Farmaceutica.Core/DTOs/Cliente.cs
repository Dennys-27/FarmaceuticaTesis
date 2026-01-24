using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Core.DTOs
{
    public class Cliente
    {
        public int cli_id { get; set; }
        public string cli_nombre { get; set; }
        public string cli_ruc { get; set; }
        public string cli_direcc { get; set; }
        public string cli_correo { get; set; }
        public string cli_telf { get; set; }
    }
}
