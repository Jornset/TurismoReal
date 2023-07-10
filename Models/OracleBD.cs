using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace TursimoReal.Models
{
    public class OracleBD
    {
        public static OracleConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            return new OracleConnection(connectionString);
        }
    }

    public class PermisosRolAtribute : ActionFilterAttribute
    {
        private Usuarios.Rol idrol;

        public PermisosRolAtribute(Usuarios.Rol _idrol)
        {
            this.idrol = _idrol;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (HttpContext.Current.Session["Rol"] != null)
            {
                Usuarios user = HttpContext.Current.Session["Rol"] as Usuarios;

                if (user.IdRol != this.idrol)
                {
                    filterContext.Result = new RedirectResult("~/Home/SinPermisos");
                }
            }
        }
    }


  
}