using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TursimoReal.Models;
using Oracle.ManagedDataAccess.Client;

namespace TursimoReal.Controllers
{
    public class Reg_UsuarioController : Controller
    {
        // GET: Reg_Usuario
        public ActionResult Registrar(Usuarios model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuarios usuario = new Usuarios();
                    usuario.RegistrarUsuario(model);

                    return RedirectToAction("Login", "LG_Usuarios"); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error durante el registro: " + ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult RegistrarUsuarios(Usuarios model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Usuarios usuario = new Usuarios();
                    usuario.RegistrarUsuario(model);

                    return RedirectToAction("Login", "LG_Usuarios"); 
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "Error durante el registro: " + ex.Message);
                }
            }

            return View(model); 
        }
    }
}