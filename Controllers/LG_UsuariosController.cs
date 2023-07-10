using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Web.Mvc;
using System.Web.Security;
using TursimoReal.Models;

namespace TursimoReal.Controllers
{
    public class LG_UsuariosController : Controller
    {
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string correo, string clave)
        {
            if (!string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(clave))
            {
                Usuarios user = Usuarios.BuscarUsuarios(correo, clave);

                if (user.Nombre != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Correo, false);

                    // Almacena el rol del usuario en la sesión en lugar del objeto de usuario completo
                    Session["Rol"] = user;

                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SinPermisos()//Redirige a una pestaña para usuarios no autorizados
        {
            ViewBag.Message = "Usted no cuenta con los permisos suficientes para acceder a esta pestaña.";

            return View();
        }

        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Session["Usuario"] = null;
            return RedirectToAction("Login", "LG_Usuarios");
        }
    }
}