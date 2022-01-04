using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plantilla_de_Correo.Class
{
    public class DatosUser
    {
        public string nombre { get; set; }
        public string registro { get; set; }
        public string correo { get; set; }
        public string cedula { get; set; }
        public string fechaVigencia { get; set; }
        public string ubicacion { get; set; }
        public string timeSpanInicio { get; set; }
        public string timeSpanFin { get; set; }
    }
}