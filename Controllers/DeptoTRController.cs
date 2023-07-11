using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TursimoReal.Models;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using Oracle.ManagedDataAccess.Client;
using System.Drawing;


namespace TursimoReal.Controllers
{
    public class DeptoTRController : Controller
    {

        //public ActionResult IndexDepto(int? page)
        //{
        //    int pageSize = 9;
        //    int pageNumber = page ?? 1;

        //    Departamentos depto = new Departamentos();

        //    // Obtener la lista completa de departamentos disponibles
        //    List<Departamentos> departamentosDisponibles = depto.ObtenerTodosLosDepartamentos();

        //    // Obtener la lista de departamentos seleccionados de la sesión
        //    List<Departamentos> departamentosSeleccionados = Session["DepartamentosSeleccionados"] as List<Departamentos>;
        //    if (departamentosSeleccionados == null)
        //    {
        //        departamentosSeleccionados = new List<Departamentos>();
        //        Session["DepartamentosSeleccionados"] = departamentosSeleccionados;
        //    }

        //    // Excluir los departamentos seleccionados de la lista completa
        //    List<Departamentos> departamentosRestantes = departamentosDisponibles.Except(departamentosSeleccionados).ToList();

        //    // Paginar la lista de departamentos restantes
        //    IPagedList<Departamentos> departamentosPaginados = departamentosRestantes.ToPagedList(pageNumber, pageSize);

        //    return View(departamentosPaginados);
        //}

        public ActionResult IndexDepto(int? page)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;

            Departamentos depto = new Departamentos();

            IPagedList<Departamentos> departamentos = depto.PaginadoDepartamentos(pageNumber);

            return View(departamentos);
        }

        #region AccesoAdmin
        [PermisosRolAtribute(Usuarios.Rol.Administrador)]
        public ActionResult EditarDepartamento(int id)
        {
            Departamentos departamentoService = new Departamentos();
            Departamentos departamento = departamentoService.ObtenerDepartamentoPorId(id);
            return View(departamento);
        }

        [PermisosRolAtribute(Usuarios.Rol.Administrador)]
        public ActionResult ActualizarDepartamento(Departamentos departamento, HttpPostedFileBase ImagenFile)
        {

            if (ModelState.IsValid)
            {
                if (ImagenFile != null && ImagenFile.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        ImagenFile.InputStream.CopyTo(ms);
                        departamento.Imagen = ms.ToArray();
                    }

                }
                else
                {
                    Departamentos departamentoServiceElse = new Departamentos();
                    // Si no se ha subido una imagen nueva, conserva la imagen existente en la base de datos
                    var departamentoExistente = departamentoServiceElse.ObtenerDepartamentoPorId(departamento.Id_depto);
                    if (departamentoExistente != null)
                    {
                        departamento.Imagen = departamentoExistente.Imagen;
                    }
                }
                Departamentos departamentoService = new Departamentos();
                departamentoService.ModificarDepartamento(departamento);
               // return RedirectToAction("About", "Home");
            }
            return View(departamento);
        }


        [PermisosRolAtribute(Usuarios.Rol.Administrador)]
        public ActionResult AdminAddDepto()
        {

            Regiones region = new Regiones();
            region.ObtenerNombreRegion(region);

            var listaRegion = region.regionesDisponibles.Select(s => new SelectListItem
            {
                Value = s.Id_region.ToString(),
                Text = s.NombreRegion,

            }).ToList();

            ViewBag.listaRegiones = listaRegion;

            //Usuarios user = new Usuarios();
            //user.ObtenerNombreUsuarios(user);

            //var listaUsuario = user.ListarUsuario.Select(s => new SelectListItem
            //{
            //    Value = s.Id_user.ToString(),
            //    Text = s.Nombre
            //}).ToList();

            //ViewBag.lisstaAdminU = listaUsuario;
            return View();

        }

        [HttpPost]
        [PermisosRolAtribute(Usuarios.Rol.Administrador)]
        public ActionResult GuardarDepartamento(Departamentos departamento, HttpPostedFileBase ImagenFile)
        {
            var valorSeleccionado = Request.Form["ddlRegion"];
            int valorSeleccionadoInt;
            if (int.TryParse(valorSeleccionado, out valorSeleccionadoInt))
            {
                // La conversión fue exitosa, se utiliza el valorSeleccionadoInt como un entero
                departamento.Region = valorSeleccionadoInt;
            }
            else
            {
                // La conversión falló, maneja el escenario de error según sea necesario
                // Por ejemplo, puedes asignar un valor predeterminado o mostrar un mensaje de error al usuario
                departamento.Region = 0; // Valor predeterminado
            }

            //var valorSeleccionadoUser = Request.Form["ddlUsuario"];
            //int valorSeleccionadoUserInt;

            //if (int.TryParse(valorSeleccionadoUser, out valorSeleccionadoUserInt))
            //{
            //    usuario.Id_user = valorSeleccionadoUserInt;
            //}
            //else
            //{
            //    departamento.Admin_Encargado = 0;
            //}

            if (ModelState.IsValid)
            {
                Departamentos departamentoService = new Departamentos();
                byte[] imagenBytes = departamentoService.Imagen;
                if (ImagenFile != null && ImagenFile.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        ImagenFile.InputStream.CopyTo(ms);
                        imagenBytes = ms.ToArray();
                    }

                }
                Regiones reg = new Regiones();

                var nuevoDepartamento = new Departamentos()
                {
                    Nombre = departamento.Nombre,
                    Direccion = departamento.Direccion,
                    Numero_Depto = departamento.Numero_Depto,
                    Descripcion = departamento.Descripcion,
                    Imagen = imagenBytes,
                    Valor_Arriendo = departamento.Valor_Arriendo,
                    Comentario = departamento.Comentario,
                    Calificacion = departamento.Calificacion,
                    Region = valorSeleccionadoInt,
                    Admin_Encargado = departamento.Admin_Encargado,
                    Disponibilidad = departamento.Disponibilidad,
                };




                departamentoService.GuardarDepartamento(nuevoDepartamento);
                nuevoDepartamento.ListarDepartamentos = departamentoService.ObtenerTodosLosDepartamentos();

                    return View("~/Views/Home/Index.cshtml");
            }
            return View(departamento);
            
        }

        public ActionResult ObtenerImagen(int id)
        {
            // Conectarse a Oracle y obtener la imagen como byte[]
            byte[] imagenBytes;
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                // Realizar la consulta para obtener la imagen desde Oracle
                string query = "SELECT imagen FROM depto WHERE id_depto = :id";
                OracleCommand command = new OracleCommand(query, connection);
                command.Parameters.Add(new OracleParameter(":id", id));

                // Ejecutar la consulta y obtener la imagen como byte[]
                OracleDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    imagenBytes = (byte[])reader["imagen"];
                }
                else
                {
                    return HttpNotFound();
                }
            }
            // Devolver la imagen como FileResult
            return File(imagenBytes, "image/png"); 
        }
        #endregion
    }
}