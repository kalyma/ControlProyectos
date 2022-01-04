using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ControlProyectos.Models.ViewModels
{
    public class TablaViewModel
    {
        
        //[Display(Name = "Id Proyecto")]
        //[MaxLength(50)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProyecto { get; set; }

        [Required]
        [Display(Name = "Código de proyecto")]
        [StringLength(50)]
        public string CodProyecto { get; set; }

        [Required]
        [Display(Name = "Nombre de proyecto")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        [StringLength(50)]
        public string Tipo { get; set; }

        [Required]
        [Display(Name = "Lider Técnico")]
        [StringLength(50)]
        public string LiderTecnico { get; set; }

        [Required]
        [Display(Name = "Gestor de Transición")]
        [StringLength(50)]
        public string GestorTransicion { get; set; }

        [Required]
        [Display(Name = "Etapa Actual")]
        [StringLength(50)]
        public string EtapaActual { get; set; }

        [Required]
        [Display(Name = "Etapa de Transición")]
        [StringLength(1000)]
        public string EtapaTransicion { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Definición Módelo Servicio")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DefinicionModeloServicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Revisión Documento Diseño")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RevisionDocumentoDiseno { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Revisión Arquitectura Servicio")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RevisionArquitecturaServicio { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Fecha Certificación a Produccion")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CertificacionPasoProduccion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Cierre Estabilizacion")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CierreEstabilizacion { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Certificacion Funcional y Técnica")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CertifFuncYtec { get; set; }



        public int? TiempoAprobacionTecnica { get; set; }

        public int? TiempoCierreEstabilizacion { get; set; }

    }
}