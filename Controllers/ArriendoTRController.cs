using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Services.Description;
using TursimoReal.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework;
using System.Reflection;
using System.Drawing;
using Microsoft.Ajax.Utilities;
using System.Configuration;
using iTextSharp.text.pdf.qrcode;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;

namespace TursimoReal.Controllers
{
    public class ArriendoTRController : Controller
    {
        #region ArriendoController
        private ArriendoDataAccess arriendoDataAccess = new ArriendoDataAccess();
        private Departamentos deptoData = new Departamentos();


        // GET: ArriendoTR
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RealizarReserva(int idDepto)
        {

            Usuarios userData = new Usuarios();
            Acompañante acompData = new Acompañante();

            Departamentos departamentoDisponible = deptoData.ObtenerDepartamentoPorId(idDepto);
            ViewBag.Departamento = departamentoDisponible;

            Arriendo arriendoData = new Arriendo();

            var valorSeleccionado = Request.Form["Datos"];
            int valorSeleccionadoInt;

            if (int.TryParse(valorSeleccionado, out valorSeleccionadoInt))
            {
                // La conversión fue exitosa, se utiliza el valorSeleccionadoInt como un entero
                arriendoData.Id_srv_ar = valorSeleccionadoInt;
            }
            else
            {
                // La conversión falló, manejar el escenario de error según sea necesario
                // Asignar un valor predeterminado o mostrar un mensaje de error al usuario
                arriendoData.Id_srv_ar = 0; // Valor predeterminado
            }


            Servicios servicio = new Servicios();
            servicio.ObtenerNombreServicio(servicio);

            var listaServicios = servicio.ServiciosDisponibles.Select(s => new SelectListItem
            {
                Value = s.Id_servicio.ToString(),
                Text = s.Nombre_servicio,
                //Text = $"{s.Nombre_servicio} |{s.Costo_servicio}",
            }).ToList();

            ViewBag.listaServicios = listaServicios;

            List<Servicios> serviciosL = servicio.ObtenerTodosLosServicios();

            Servicios servicioAdd = new Servicios
            {
                Id_servicio = servicio.Id_servicio,
                Nombre_servicio = servicio.Nombre_servicio,
                Descripcion = servicio.Descripcion,
                Disponible = servicio.Disponible,
                Count_disponibilidad = servicio.Count_disponibilidad,
                Costo_servicio = servicio.Costo_servicio
            };
            serviciosL.Add(servicioAdd);

            ViewBag.Servicios = serviciosL;

            arriendoData.Estado_pago = "N";
            
            // Obtener la disponibilidad del departamento
            char disponibilidad = arriendoDataAccess.ObtenerDisponibilidad(departamentoDisponible.Id_depto);
            if (disponibilidad == 'S')
            {
                Arriendo arriendo = new Arriendo
                {
                    Fecha_ini = DateTime.Now,
                    Fecha_termi = arriendoData.Fecha_termi,
                    Valor_dia = arriendoData.Valor_dia,
                    Total_pago = departamentoDisponible.Valor_Arriendo,
                    Metodo_pago = arriendoData.Metodo_pago,
                    Estado_pago = arriendoData.Estado_pago,
                    Id_users = userData.Id_user,
                    Id_deptos = departamentoDisponible.Id_depto,
                    Id_srv_ar = valorSeleccionadoInt,
                    Id_acomp = acompData.Id_Acompannante
                };

                return View(arriendo);

            }

            //var modeloJson = JsonSerializer.Serialize(arriendo);
            //ViewBag.ModeloJson = modeljson;

            return View();
        }

        [HttpPost]
        public JsonResult RealizarReservaPost(string data)
        {
            Arriendo arriendo = new Arriendo();
            var arriendo2 = JsonConvert.DeserializeObject<Object>(data);
            arriendo = JsonConvert.DeserializeObject<Arriendo>(data);

            //foreach (var propiedad in typeof(Arriendo).GetProperties())
            //{
            //    var valor = propiedad.GetValue(arriendo2);
            //}

           

            arriendoDataAccess.RealizarArriendo(arriendo.Fecha_ini, arriendo.Fecha_termi, arriendo.Valor_dia, arriendo.Total_pago, arriendo.Metodo_pago,
                arriendo.Estado_pago, (int)arriendo.Id_users, (int)arriendo.Id_deptos, (int)arriendo.Id_srv_ar, (int)arriendo.Id_acomp);

            return Json("ReservaExitosa", "ArriendoTR");
        }
        #region CodigoComentado
        //public ActionResult RealizarReserva(int idDepto)
        //{

        //    Departamentos departamentoDisponible = deptoData.ObtenerDepartamentoPorId(idDepto);
        //    ViewBag.Departamento = departamentoDisponible;

        //    Servicios servicio = new Servicios();
        //    servicio.ObtenerNombreServicio(servicio);


        //    var listaServicios = servicio.ServiciosDisponibles.Select(s => new SelectListItem
        //    {
        //        Value = $"{s.Id_servicio}|{s.Costo_servicio}",
        //        Text = s.Nombre_servicio,

        //    }).ToList();


        //    ViewBag.listaServicios = listaServicios;

        //    Usuarios userData = new Usuarios();
        //    Acompañante acompData = new Acompañante();
        //    Arriendo arriendoData = new Arriendo();

        //    var valorSeleccionado = Request.Form["servicioId"];
        //    int valorSeleccionadoInt;


        //    if (int.TryParse(valorSeleccionado, out valorSeleccionadoInt))
        //    {
        //        // La conversión fue exitosa, se utiliza el valorSeleccionadoInt como un entero
        //        arriendoData.Id_srv_ar = valorSeleccionadoInt;
        //    }
        //    else
        //    {
        //        // La conversión falló, maneja el escenario de error según sea necesario
        //        // Por ejemplo, puedes asignar un valor predeterminado o mostrar un mensaje de error al usuario
        //        arriendoData.Id_srv_ar = 0; // Valor predeterminado
        //    }

        //    // Obtener la disponibilidad del departamento
        //    char disponibilidad = arriendoDataAccess.ObtenerDisponibilidad(departamentoDisponible.Id_depto);
        //    if (disponibilidad == 'S')
        //    {
        //        Arriendo arriendo = new Arriendo
        //        {
        //            Fecha_ini = DateTime.Now,
        //            Fecha_termi = arriendoData.Fecha_termi,
        //            Valor_dia = arriendoData.Valor_dia,
        //            Total_pago = departamentoDisponible.Valor_Arriendo,
        //            Metodo_pago = arriendoData.Metodo_pago,
        //            Estado_pago = arriendoData.Estado_pago,
        //            Id_users = userData.Id_user,
        //            Id_deptos = departamentoDisponible.Id_depto,
        //            Id_srv_ar = valorSeleccionadoInt,
        //            Id_acomp = acompData.Id_Acompannante
        //        };



        //        return View(arriendo);
        //    }
        //    return View();
        //}


        #region PrimeraPruebaDeArriendo
        //public ActionResult RealizarReserva(int idDepto)
        //{
        //    List<Arriendo> modelo = new List<Arriendo>();
        //    List<Departamentos> departamentosDisponibles = deptoData.ObtenerListaDepartamentosPorId(idDepto);

        //    // - Obtener la lista de servicios disponibles y pasarla al modelo
        //    List<Servicios> Listservicios = new List<Servicios>();
        //    // Lógica para obtener la lista de servicios disponibles

        //    // - Obtener otros datos necesarios para el formulario de reserva
        //    Arriendo arriendoData = new Arriendo();
        //    Usuarios userData = new Usuarios();
        //    Acompañante acompData = new Acompañante();

        //    foreach (Departamentos departamento in departamentosDisponibles)
        //    {
        //        ViewBag.Departamento = departamento;
        //        // Obtener la disponibilidad del departamento
        //        char disponibilidad = arriendoDataAccess.ObtenerDisponibilidad(departamento.Id_depto);
        //        if (disponibilidad == 'S')
        //        {
        //            foreach (Servicios servicio in Listservicios)
        //            {
        //                ViewBag.Servicio = servicio;
        //                Arriendo arriendo = new Arriendo
        //                {
        //                    Fecha_ini = DateTime.Now,
        //                    Fecha_termi = arriendoData.Fecha_termi,
        //                    Valor_dia = arriendoData.Valor_dia,
        //                    Total_pago = departamento.Valor_Arriendo,
        //                    Metodo_pago = arriendoData.Metodo_pago,
        //                    Estado_pago = arriendoData.Estado_pago,
        //                    Id_users = userData.Id_user,
        //                    Id_deptos = departamento.Id_depto,
        //                    Id_srv_ar = servicio.Id_servicio,
        //                    Id_acomp = acompData.Id_Acompannante
        //                };

        //                modelo.Add(arriendo);
        //            }
        //        }
        //    }

        //    return View(modelo);
        //}

        #endregion

        //[HttpPost]
        //public ActionResult RealizarReserva(Arriendo arriendo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Lógica de negocio para validar la disponibilidad y guardar la reserva
        //        char disponibilidad = arriendoDataAccess.ObtenerDisponibilidad(arriendo.Id_deptos.Value);
        //        if (disponibilidad == 'S')
        //        {
        //            // Calcular el valor del pago, generar ID de registro, etc.
        //            int totalPago = CalcularTotalPago(arriendo.Fecha_ini, arriendo.Fecha_termi.Value, arriendo.Valor_dia);
        //            string estadoPago = "Pendiente";

        //            // Llamar al método arriendoDataAccess.RealizarArriendo para guardar la reserva
        //            arriendoDataAccess.RealizarArriendo(
        //                arriendo.Fecha_ini, arriendo.Fecha_termi.Value, arriendo.Valor_dia, totalPago,
        //                arriendo.Metodo_pago, estadoPago, arriendo.Id_users.Value, arriendo.Id_deptos.Value, arriendo.Id_srv_ar.Value,
        //                arriendo.Id_acomp.Value);

        //            // Redirigir a  página de éxito o mostrar un mensaje de éxito
        //            return RedirectToAction("ReservaExitosa");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "El departamento seleccionado está ocupado. Por favor, elige otro departamento.");
        //        }
        //    }

        //    // Si hay errores de validación, volver a mostrar el formulario con los errores
        //    return View(arriendo);
        //} 
        #endregion

        public ActionResult ReservaExitosa()
        {
            // Lógica para mostrar una página de éxito después de realizar la reserva
            return View();
        }

        private int CalcularTotalPago(DateTime fechaInicio, DateTime fechaTermino, int valorDia)
        {
            // Lógica para calcular el total a pagar en base a las fechas de inicio y término y el valor por día
            TimeSpan duracion = fechaTermino - fechaInicio;
            int dias = duracion.Days;
            int totalPago = valorDia * dias;
            return totalPago;
        }
        #endregion


        #region ServicioController
        public ActionResult RegistrarServicio(Servicios servicios)
        {
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                using (OracleCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SP_REGISTRAR_SERVICIO";
                    command.Parameters.Add("p_id_servicio", OracleDbType.Int32).Value = servicios.Id_servicio;
                    command.Parameters.Add("p_nombre_servicio", OracleDbType.Varchar2).Value = servicios.Nombre_servicio;
                    command.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = servicios.Descripcion;
                    command.Parameters.Add("p_disponible", OracleDbType.Varchar2).Value = servicios.Disponible;
                    command.Parameters.Add("p_count_disponibilidad", OracleDbType.Int32).Value = servicios.Count_disponibilidad;
                    command.Parameters.Add("p_costo_servicio", OracleDbType.Int32).Value = servicios.Costo_servicio;

                    command.ExecuteNonQuery();
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditarServicio(int id)
        {
            Servicios serv = new Servicios();
            Servicios servicio = serv.ObtenerServicioPorId(id);
            return View(servicio);
        } 
        #endregion

        private List<SelectListItem> GetNombreServicio()
        {
            List<SelectListItem> listaServicios = new List<SelectListItem>();

            using (OracleConnection cn = OracleBD.GetConnection())
            {
                cn.Open();

                using (OracleCommand command = new OracleCommand("SELECT id_servicio,nombre_servicio FROM SERVICIO", cn))
                {

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idServicio = Convert.ToInt32(reader["Id_servicio"]);
                            string nombreServicio = reader["Nombre_servicio"].ToString();

                            SelectListItem item = new SelectListItem
                            {
                                Value = idServicio.ToString(),
                                Text = nombreServicio
                            };

                            listaServicios.Add(item);
                        }
                    }
                }
            }
            return listaServicios;
        }
    }
}