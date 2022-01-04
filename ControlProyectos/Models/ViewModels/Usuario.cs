using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControlProyectos.Class
{
    public class Usuario
    {

            public Usuario()
            {
                //this.Id = Guid.NewGuid();
            }

        [Key]
        [Display(Name = "Usuario")]
        [Required(ErrorMessage ="Digite el usuario")]
        public String Id { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Contraseña es requerida")]
        public string Contrasena { get; set; }

    }

    







}