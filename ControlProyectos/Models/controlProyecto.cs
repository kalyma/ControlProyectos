//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControlProyectos.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class controlProyecto
    {
        public int idProyecto { get; set; }
        public string codProyecto { get; set; }
        public string nombre { get; set; }
        public string tipo { get; set; }
        public string liderTecnico { get; set; }
        public string gestorTransicion { get; set; }
        public string etapaActual { get; set; }
        public string etapaTransicion { get; set; }
        public Nullable<System.DateTime> definicionModeloServicio { get; set; }
        public Nullable<System.DateTime> revisionDocumentoDiseno { get; set; }
        public Nullable<System.DateTime> revisionArquitecturaServicio { get; set; }
        public Nullable<System.DateTime> certificacionPasoProduccion { get; set; }
        public Nullable<System.DateTime> cierreEstabilizacion { get; set; }
        public Nullable<System.DateTime> certiFuncYtec { get; set; }
    }
}
