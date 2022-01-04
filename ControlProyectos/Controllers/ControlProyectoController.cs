using ControlProyectos.Class;
using ControlProyectos.Models;
using ControlProyectos.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControlProyectos.Controllers
{
    public class ControlProyectoController : Controller
    {
        // GET: ControlProyecto
        public ActionResult Inicio()
        {

            return View();

        }

        [HttpPost]
        public ActionResult Inicio(string usuario, string contraseña)
        {
            String usuarioLogin = Request["txtUsuario"];
            String passLogin = Request["txtPass"];
            Console.WriteLine(usuarioLogin + " " + passLogin);
            return View();

        }

        public ActionResult Index(string buscar, string liderTecnico, string gestorTransicion, string buscafecha, string buscafecha1)
        {
            if (liderTecnico != null && liderTecnico != "") 
            { 
                string liderTecnico1 = liderTecnico;
                string liderTecnico12 = liderTecnico1.Replace("{", "");
                string liderTecnico13 = liderTecnico12.Replace("}", "");
                liderTecnico = liderTecnico13;
            }

            if (gestorTransicion != null && gestorTransicion != "")
            {
                string gestorTransicion1 = gestorTransicion;
                string gestorTransicion12 = gestorTransicion1.Replace("{", "");
                string gestorTransicion13 = gestorTransicion12.Replace("}", "");
                gestorTransicion = gestorTransicion13;
            }

            DateTime fechaFormateada = new DateTime();

            if (buscafecha != null && buscafecha != "") 
            { 
                DateTime.TryParse(buscafecha, out fechaFormateada);
            }

            if (buscafecha1 != null && buscafecha1 != "")
            {
                DateTime.TryParse(buscafecha1, out fechaFormateada);
            }



            if (buscar == null && liderTecnico == null && gestorTransicion == null && buscafecha == null && buscafecha1 == null)
            {
                if (Session["user"] != null)
                {
                    List<TablaViewModel> lst;



                    using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())                  
                    {
                        lst = (from d in db.controlProyecto
                               select new TablaViewModel
                               {
                                   IdProyecto = d.idProyecto,
                                   CodProyecto = d.codProyecto,
                                   Nombre = d.nombre,
                                   Tipo = d.tipo,
                                   LiderTecnico = d.liderTecnico,
                                   GestorTransicion = d.gestorTransicion,
                                   EtapaActual = d.etapaActual,
                                   EtapaTransicion = d.etapaTransicion,
                                   //Campos Fecha
                                   RevisionDocumentoDiseno = d.revisionDocumentoDiseno,
                                   DefinicionModeloServicio = d.definicionModeloServicio,
                                   RevisionArquitecturaServicio = d.revisionArquitecturaServicio,
                                   CertifFuncYtec = d.certiFuncYtec,
                                   CertificacionPasoProduccion = d.certificacionPasoProduccion,
                                   CierreEstabilizacion = d.cierreEstabilizacion,
                                   TiempoAprobacionTecnica = System.Data.Entity.DbFunctions.DiffDays(d.definicionModeloServicio.Value, d.certificacionPasoProduccion.Value),
                                   //TiempoAprobacionTecnica =  d.certificacionPasoProduccion.Value - d.definicionModeloServicio.Value.Day,
                                   TiempoCierreEstabilizacion = System.Data.Entity.DbFunctions.DiffDays(d.certificacionPasoProduccion.Value, d.cierreEstabilizacion.Value)

                               }).ToList();
                    }
                    ViewBag.UsuarioConectado = Session["user"];

                    return View(lst);
                }
                else
                {
                    return Redirect("~/ControlProyecto");
                }
            }
            else
            {
                //ViewBag.NameSortParm = string.IsNullOrEmpty(buscar) ? "name_desc" : "";
                //ViewBag.DateSortParm = buscar == "Date" ? "date_desc" : "Date";

                if(liderTecnico != null && liderTecnico != "") 
                {
                    List<TablaViewModel> lst;

                    using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                    {
                        lst = (from d in db.controlProyecto
                               select new TablaViewModel
                               {
                                   IdProyecto = d.idProyecto,
                                   CodProyecto = d.codProyecto,
                                   Nombre = d.nombre,
                                   Tipo = d.tipo,
                                   LiderTecnico = d.liderTecnico,
                                   GestorTransicion = d.gestorTransicion,
                                   EtapaActual = d.etapaActual,
                                   EtapaTransicion = d.etapaTransicion,
                                   //Campos Fecha
                                   RevisionDocumentoDiseno = d.revisionDocumentoDiseno,
                                   DefinicionModeloServicio = d.definicionModeloServicio,
                                   RevisionArquitecturaServicio = d.revisionArquitecturaServicio,
                                   CertifFuncYtec = d.certiFuncYtec,
                                   CertificacionPasoProduccion = d.certificacionPasoProduccion,
                                   CierreEstabilizacion = d.cierreEstabilizacion,
                                   TiempoAprobacionTecnica = System.Data.Entity.DbFunctions.DiffDays(d.definicionModeloServicio.Value, d.certificacionPasoProduccion.Value),
                                   TiempoCierreEstabilizacion = System.Data.Entity.DbFunctions.DiffDays(d.certificacionPasoProduccion.Value, d.cierreEstabilizacion.Value)

                               }).ToList();

                        lst = lst.Where(t => t.LiderTecnico == liderTecnico).ToList();
                    }

                    return View(lst);
                }

                if (buscar != null && buscar != "") /*&& liderTecnico == null && gestorTransicion == null && buscafecha == null)*/
                {
                    List<TablaViewModel> lst;

                    using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                    {
                        lst = (from d in db.controlProyecto
                               select new TablaViewModel
                               {
                                  IdProyecto = d.idProyecto,
                                   CodProyecto = d.codProyecto,
                                   Nombre = d.nombre,
                                   Tipo = d.tipo,
                                   LiderTecnico = d.liderTecnico,
                                   GestorTransicion = d.gestorTransicion,
                                   EtapaActual = d.etapaActual,
                                   EtapaTransicion = d.etapaTransicion,
                                   //Campos Fecha
                                   RevisionDocumentoDiseno = d.revisionDocumentoDiseno,
                                   DefinicionModeloServicio = d.definicionModeloServicio,
                                   RevisionArquitecturaServicio = d.revisionArquitecturaServicio,
                                   CertifFuncYtec = d.certiFuncYtec,
                                   CertificacionPasoProduccion = d.certificacionPasoProduccion,
                                   CierreEstabilizacion = d.cierreEstabilizacion,
                                   TiempoAprobacionTecnica = System.Data.Entity.DbFunctions.DiffDays(d.definicionModeloServicio.Value, d.certificacionPasoProduccion.Value),
                                   TiempoCierreEstabilizacion = System.Data.Entity.DbFunctions.DiffDays(d.certificacionPasoProduccion.Value, d.cierreEstabilizacion.Value)
                               }).ToList();

                        lst = lst.FindAll((t => t.CodProyecto.Contains(buscar))).ToList();
                    }
                    return View(lst);
                }
                
                if (gestorTransicion != null && gestorTransicion != "")
                {
                    List<TablaViewModel> lst;

                    using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                    {
                        lst = (from d in db.controlProyecto
                               select new TablaViewModel
                               {
                                   IdProyecto = d.idProyecto,
                                   CodProyecto = d.codProyecto,
                                   Nombre = d.nombre,
                                   Tipo = d.tipo,
                                   LiderTecnico = d.liderTecnico,
                                   GestorTransicion = d.gestorTransicion,
                                   EtapaActual = d.etapaActual,
                                   EtapaTransicion = d.etapaTransicion,
                                   //Campos Fecha
                                   RevisionDocumentoDiseno = d.revisionDocumentoDiseno,
                                   DefinicionModeloServicio = d.definicionModeloServicio,
                                   RevisionArquitecturaServicio = d.revisionArquitecturaServicio,
                                   CertifFuncYtec = d.certiFuncYtec,
                                   CertificacionPasoProduccion = d.certificacionPasoProduccion,
                                   CierreEstabilizacion = d.cierreEstabilizacion,
                                   TiempoAprobacionTecnica = System.Data.Entity.DbFunctions.DiffDays(d.definicionModeloServicio.Value, d.certificacionPasoProduccion.Value),
                                   TiempoCierreEstabilizacion = System.Data.Entity.DbFunctions.DiffDays(d.certificacionPasoProduccion.Value, d.cierreEstabilizacion.Value)
                               }).ToList();

                        lst = lst.Where(t => t.GestorTransicion == gestorTransicion).ToList();
                    }
                    return View(lst);
                }

                if (buscafecha != null && buscafecha != "")
                {
                    List<TablaViewModel> lst;

                    using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                    {
                        lst = (from d in db.controlProyecto
                               select new TablaViewModel
                               {
                                   IdProyecto = d.idProyecto,
                                   CodProyecto = d.codProyecto,
                                   Nombre = d.nombre,
                                   Tipo = d.tipo,
                                   LiderTecnico = d.liderTecnico,
                                   GestorTransicion = d.gestorTransicion,
                                   EtapaActual = d.etapaActual,
                                   EtapaTransicion = d.etapaTransicion,
                                   //Campos Fecha
                                   RevisionDocumentoDiseno = d.revisionDocumentoDiseno,
                                   DefinicionModeloServicio = d.definicionModeloServicio,
                                   RevisionArquitecturaServicio = d.revisionArquitecturaServicio,
                                   CertifFuncYtec = d.certiFuncYtec,
                                   CertificacionPasoProduccion = d.certificacionPasoProduccion,
                                   CierreEstabilizacion = d.cierreEstabilizacion,
                                   TiempoAprobacionTecnica = System.Data.Entity.DbFunctions.DiffDays(d.definicionModeloServicio.Value, d.certificacionPasoProduccion.Value),
                                   TiempoCierreEstabilizacion = System.Data.Entity.DbFunctions.DiffDays(d.certificacionPasoProduccion.Value, d.cierreEstabilizacion.Value)
                               }).ToList();

                        //lst = lst.FindAll((t => t.CodProyecto.Contains(buscar))).ToList();
                        //lst = lst.FindAll((t => t.DefinicionModeloServicio.Contains(fec   haFormateada))).ToList();

                        //lst = lst.Where(t => t.DefinicionModeloServicio.Value.Year == fechaFormateada.Year).ToList();
                        lst = lst.Where(t => t.DefinicionModeloServicio.Value.Year == fechaFormateada.Year && t.DefinicionModeloServicio.Value.Month == fechaFormateada.Month && t.DefinicionModeloServicio.Value.Day == fechaFormateada.Day).ToList();
                    }
                    return View(lst);
                }

                if (buscafecha1 != null && buscafecha1 != "")
                {
                    List<TablaViewModel> lst;

                    using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                    {
                        lst = (from d in db.controlProyecto
                               select new TablaViewModel
                               {
                                   IdProyecto = d.idProyecto,
                                   CodProyecto = d.codProyecto,
                                   Nombre = d.nombre,
                                   Tipo = d.tipo,
                                   LiderTecnico = d.liderTecnico,
                                   GestorTransicion = d.gestorTransicion,
                                   EtapaActual = d.etapaActual,
                                   EtapaTransicion = d.etapaTransicion,
                                   //Campos Fecha
                                   RevisionDocumentoDiseno = d.revisionDocumentoDiseno,
                                   DefinicionModeloServicio = d.definicionModeloServicio,
                                   RevisionArquitecturaServicio = d.revisionArquitecturaServicio,
                                   CertifFuncYtec = d.certiFuncYtec,
                                   CertificacionPasoProduccion = d.certificacionPasoProduccion,
                                   CierreEstabilizacion = d.cierreEstabilizacion,
                                   TiempoAprobacionTecnica = System.Data.Entity.DbFunctions.DiffDays(d.definicionModeloServicio.Value, d.certificacionPasoProduccion.Value),
                                   TiempoCierreEstabilizacion = System.Data.Entity.DbFunctions.DiffDays(d.certificacionPasoProduccion.Value, d.cierreEstabilizacion.Value)
                               }).ToList();

                        lst = lst.Where(t => t.CertifFuncYtec.Value.Year == fechaFormateada.Year && t.CertifFuncYtec.Value.Month == fechaFormateada.Month && t.CertifFuncYtec.Value.Day == fechaFormateada.Day).ToList();
                    }
                    return View(lst);
                }
                else
                {
                    return Redirect("~/ControlProyecto/Index");
                }
            }

        }

        public int calcularFecha(DateTime fecha)
        {
            int dias = 0;
            dias = fecha.Subtract(DateTime.Today).Days;

            return dias;
        }

        public ActionResult Nuevo()
        {

            if (Session["user"] != null)
            {
                return View();
            }
            else
            {
                return Redirect("~/ControlProyecto/");
            }
        }

        [HttpPost]
        public ActionResult Nuevo(TablaViewModel model)
        {
            if (Session["user"] != null)
            {
                try
                {
                    if(model.DefinicionModeloServicio > model.RevisionDocumentoDiseno)
                    {
                        ViewBag.successMessage = "La fecha de Definición no debe ser mayor a la fecha de Revisión";
                        return View(model);
                    }
                    if (model.RevisionDocumentoDiseno > model.RevisionArquitecturaServicio)
                    {
                        ViewBag.successMessage = "La fecha Revisión Documento no debe ser mayor a la fecha de Revisión Arquitectura";
                        return View(model);
                    }
                    if (model.RevisionArquitecturaServicio > model.CertifFuncYtec)
                    {
                        ViewBag.successMessage = "La fecha Revisión Arquitectura no debe ser mayor a la fecha de Certificación Funcional";
                        return View(model);
                    }
                    if (model.CertifFuncYtec > model.CertificacionPasoProduccion)
                    {
                        ViewBag.successMessage = "La fecha de Certificación Funcional no debe ser mayor a la fecha de Paso a Producción";
                        return View(model);
                    }
                    if (model.CertificacionPasoProduccion > model.CierreEstabilizacion)
                    {
                        ViewBag.successMessage = "La fecha de Paso a Producción no debe ser mayor a la fecha de Cierre de Estabilizacion";
                        return View(model);
                    }
                   

                    List<TablaViewModel> lst;
                    using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                    {
                        lst = (from d in db.controlProyecto
                               select new TablaViewModel
                               {
                                  //IdProyecto = d.idProyecto,
                                   CodProyecto = d.codProyecto,
                                   Nombre = d.nombre,
                                   Tipo = d.tipo,
                                   LiderTecnico = d.liderTecnico,
                                   GestorTransicion = d.gestorTransicion,
                                   EtapaActual = d.etapaActual,
                                   EtapaTransicion = d.etapaTransicion,
                                   //Campos Fecha
                                   RevisionDocumentoDiseno = d.revisionDocumentoDiseno,
                                   DefinicionModeloServicio = d.definicionModeloServicio,
                                   RevisionArquitecturaServicio = d.revisionArquitecturaServicio,
                                   CertifFuncYtec = d.certiFuncYtec,
                                   CertificacionPasoProduccion = d.certificacionPasoProduccion,
                                   CierreEstabilizacion = d.cierreEstabilizacion,
                                   TiempoAprobacionTecnica = System.Data.Entity.DbFunctions.DiffDays(d.definicionModeloServicio.Value, d.certificacionPasoProduccion.Value),
                                   TiempoCierreEstabilizacion = System.Data.Entity.DbFunctions.DiffDays(d.certificacionPasoProduccion.Value, d.cierreEstabilizacion.Value)
                               }).ToList();


                        lst = lst.Where(t => t.CodProyecto == model.CodProyecto).ToList();
                    }

                    if (lst.Count > 0)
                    {
                         ViewBag.successMessage = "El código que esta ingresando ya existe";
                         return View(model);
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                            {

                                var oTabla = new controlProyecto();
                               // oTabla.idProyecto = model.IdProyecto;
                                oTabla.codProyecto = model.CodProyecto;
                                oTabla.nombre = model.Nombre;
                                oTabla.tipo = model.Tipo;
                                oTabla.liderTecnico = model.LiderTecnico;
                                oTabla.gestorTransicion = model.GestorTransicion;
                                oTabla.etapaActual = model.EtapaActual;
                                oTabla.etapaTransicion = model.EtapaTransicion;
                                oTabla.definicionModeloServicio = model.DefinicionModeloServicio;
                                oTabla.revisionDocumentoDiseno = model.RevisionDocumentoDiseno;
                                oTabla.revisionArquitecturaServicio = model.RevisionArquitecturaServicio;
                                oTabla.certificacionPasoProduccion = model.CertificacionPasoProduccion;
                                oTabla.cierreEstabilizacion = model.CierreEstabilizacion;
                                oTabla.certiFuncYtec = model.CertifFuncYtec;
                                db.controlProyecto.Add(oTabla);
                                db.SaveChanges();
                            }
                            return Redirect("~/ControlProyecto/Index");
                        }

                        return View(model);

                    }

                    
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                return Redirect("~/ControlProyecto/");
            }
        }

        //Editar

        public ActionResult Editar(int id)
        {
            if (Session["user"] != null)
            {
                String ide = Session["user"].ToString();
                //int valor = 0;
                //valor = Convert.ToInt16(id);
                TablaViewModel model = new TablaViewModel();
                using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                {
                    var oTabla = db.controlProyecto.Find(id);
                    model.IdProyecto = oTabla.idProyecto;
                    model.CodProyecto = oTabla.codProyecto;
                    model.Nombre = oTabla.nombre;
                    model.Tipo = oTabla.tipo;
                    model.LiderTecnico = oTabla.liderTecnico;
                    model.GestorTransicion = oTabla.gestorTransicion;
                    model.EtapaActual = oTabla.etapaActual;
                    model.EtapaTransicion = oTabla.etapaTransicion;

                    model.DefinicionModeloServicio = oTabla.definicionModeloServicio;
                    model.RevisionDocumentoDiseno = oTabla.revisionDocumentoDiseno;
                    model.RevisionArquitecturaServicio = oTabla.revisionArquitecturaServicio;
                    model.CertificacionPasoProduccion = oTabla.certificacionPasoProduccion;
                    model.CierreEstabilizacion = oTabla.cierreEstabilizacion;
                    model.CertifFuncYtec = oTabla.certiFuncYtec;
                }
                return View(model);
            }
            else
            {
                return Redirect("~/ControlProyecto/");
            }
        }

        [HttpPost]
        public ActionResult Editar(TablaViewModel model)
        {
            if (Session["user"] != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.DefinicionModeloServicio > model.RevisionDocumentoDiseno)
                        {
                            ViewBag.successMessage = "La fecha de Definición no debe ser mayor a la fecha de Revisión";
                            return View(model);
                        }
                        if (model.RevisionDocumentoDiseno > model.RevisionArquitecturaServicio)
                        {
                            ViewBag.successMessage = "La fecha Revisión Documento no debe ser mayor a la fecha de Revisión Arquitectura";
                            return View(model);
                        }
                        if (model.RevisionArquitecturaServicio > model.CertifFuncYtec)
                        {
                            ViewBag.successMessage = "La fecha Revisión Arquitectura no debe ser mayor a la fecha de Certificación Funcional";
                            return View(model);
                        }
                        if (model.CertifFuncYtec > model.CertificacionPasoProduccion)
                        {
                            ViewBag.successMessage = "La fecha de Certificación Funcional no debe ser mayor a la fecha de Paso a Producción";
                            return View(model);
                        }
                        if (model.CertificacionPasoProduccion > model.CierreEstabilizacion)
                        {
                            ViewBag.successMessage = "La fecha de Paso a Producción no debe ser mayor a la fecha de Cierre de Estabilizacion";
                            return View(model);
                        }

                        using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                        {

                            var horaa = System.DateTime.Now;


                            var oTabla = db.controlProyecto.Find(model.IdProyecto);
                            oTabla.idProyecto = model.IdProyecto;
                            oTabla.codProyecto = model.CodProyecto;
                            oTabla.nombre = model.Nombre;
                            oTabla.tipo = model.Tipo;
                            oTabla.liderTecnico = model.LiderTecnico;
                            oTabla.gestorTransicion = model.GestorTransicion;
                            oTabla.etapaActual = model.EtapaActual;
                            oTabla.etapaTransicion = model.EtapaTransicion;
                            oTabla.definicionModeloServicio = model.DefinicionModeloServicio;
                            oTabla.revisionDocumentoDiseno = model.RevisionDocumentoDiseno;
                            oTabla.revisionArquitecturaServicio = model.RevisionArquitecturaServicio;
                            oTabla.certificacionPasoProduccion = model.CertificacionPasoProduccion;
                            oTabla.cierreEstabilizacion = model.CierreEstabilizacion;
                            oTabla.certiFuncYtec = model.CertifFuncYtec;
                            db.Entry(oTabla).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                        }
                        return Redirect("~/ControlProyecto/Index");
                    }

                    return View(model);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                return Redirect("~/ControlProyecto/");

            }
        }
        //Eliminar
        [HttpGet]
        public ActionResult Eliminar(String id)
        {
            //onclientclick = "return confirm('¿Desea saber la hora?');" Text = "Button" />
            try
            {
                String ide = Session["user"].ToString();
                int valor = 0;
                valor = Convert.ToInt16(id);

                TablaViewModel model = new TablaViewModel();
                using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                {
                    var oTabla = db.controlProyecto.Find(valor);
                    db.controlProyecto.Remove(oTabla);
                    db.SaveChanges();

                }
                return Redirect("~/ControlProyecto/Index");

            }
            catch (Exception ex)
            {
                //string msg = ex
                return Redirect("~/ControlProyecto/");

            }

        }

        public ActionResult Login()
        {
            //return Redirect("~/ControlProyecto/");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult validarLogin(Usuario usuario)
        {
              if (ModelState.IsValid)
            {
                String _Id = usuario.Id;
                string _Contrasena = usuario.Contrasena;
                //LoginAD loginAD = new LoginAD();
                var validacion = true; // loginAD.ValidateCredentials(_Id, _Contrasena);

                if (validacion)
                {

                    Session["user"] = _Id;
                    //Session.Remove["user"];
                    return Redirect("~/ControlProyecto/Index/");


                }

                else
                {
                    ViewBag.mensajeNoConexion = "No es posible conectarse en este momento.";
                    return Redirect("~/ControlProyecto/Login/");
                }


            }
            return Redirect("~/ControlProyecto/");
            //return View();
        }

        //public ViewResult Index(string estado, int? page)
        //{

        //    var estados = db.Representantes.OrderBy(p => p.EstadosT.Name).Select(p => p.EstadosT.Name).Distinct();

        //    if (!String.IsNullOrEmpty(estado))
        //    {
        //        representante = db.Representantes.Where(s => s.EstadosT.Name.Contains(estado));

        //    }

        //    ViewBag.EstadosT = new SelectList(estados);
        //    return View(representante.ToList().ToPagedList(pageNumber, pageSize));
        //}

        public ActionResult Enviar(string id)
        {
            using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
            {
                var oTabla = db.controlProyecto.Find(id);
                var students = from s in db.controlProyecto
                               select s;
                //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
                //switch (sortOrder)
                //            {
                //                case "name_desc":
                //                    students = students.OrderByDescending(s => s.liderTecnico);
                //                    break;
                //                case "Date":
                //                    students = students.OrderBy(s => s.gestorTransicion);
                //                    break;
                //                case "date_desc":
                //                    students = students.OrderByDescending(s => s.definicionModeloServicio);
                //                    break;
                //                default:
                //                    students = students.OrderBy(s => s.etapaTransicion);
                //                    break;
                //            }
                //            return View(students.ToList());
            }

            return Redirect("~/ControlProyecto/Index");
        }

        public ActionResult Enviar1()
        {
            if (Session["user"] != null)
            {
                List<TablaViewModel> lst;

                using (TableroControlProyectosEntities1 db = new TableroControlProyectosEntities1())
                {
                    lst = (from d in db.controlProyecto
                           select new TablaViewModel
                           {
                               CodProyecto = d.codProyecto,
                               Nombre = d.nombre,
                               Tipo = d.tipo,
                               LiderTecnico = d.liderTecnico,
                               GestorTransicion = d.gestorTransicion,
                               EtapaActual = d.etapaActual,
                               EtapaTransicion = d.etapaTransicion,
                               //Campos Fecha
                               RevisionDocumentoDiseno = d.revisionDocumentoDiseno,
                               DefinicionModeloServicio = d.definicionModeloServicio,
                               RevisionArquitecturaServicio = d.revisionArquitecturaServicio,
                               CertifFuncYtec = d.certiFuncYtec,
                               CertificacionPasoProduccion = d.certificacionPasoProduccion,
                               CierreEstabilizacion = d.cierreEstabilizacion,
                               TiempoAprobacionTecnica = System.Data.Entity.DbFunctions.DiffDays(d.definicionModeloServicio.Value, d.certificacionPasoProduccion.Value),
                               TiempoCierreEstabilizacion = System.Data.Entity.DbFunctions.DiffDays(d.certificacionPasoProduccion.Value, d.cierreEstabilizacion.Value)
                           }).ToList();
                }
                return View(lst);
            }
            else
            {
                return Redirect("~/ControlProyecto/");
            }
        }
    }
}