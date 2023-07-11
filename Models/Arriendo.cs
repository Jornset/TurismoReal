using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Oracle.ManagedDataAccess.Client;
using TursimoReal.Models;
using System.Data.Entity;
using Oracle.ManagedDataAccess.EntityFramework;
using System.Web.Services.Description;
using System.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace TursimoReal.Models
{
    public class Arriendo
    {
        [Key]
        public int Id_registro { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateTime Fecha_ini { get; set; }

        [Display(Name = "Fecha Término")]
        public DateTime? Fecha_termi { get; set; }

        [Display(Name = "Valor Día")]
        public int Valor_dia { get; set; }

        [Display(Name = "Total Pago")]
        public int Total_pago { get; set; }

        [Display(Name = "Método de Pago")]
        public string Metodo_pago { get; set; }

        [Display(Name = "Estado de Pago")]
        public string Estado_pago { get; set; }

        [Display(Name = "ID Usuario")]
        public int? Id_users { get; set; }

        [Display(Name = "ID Departamento")]
        public int? Id_deptos { get; set; }

        [Display(Name = "ID Servicio")]
        public int? Id_srv_ar { get; set; }

        [Display(Name = "ID Acompañante")]
        public int? Id_acomp { get; set; }

        public List<Arriendo> ListaArriendos { get; set; }
        public List<Departamentos> DepartamentosSeleccionados { get; set; }
    }

    //public class ArriendoDataContext : DbContext
    //{
    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.HasDefaultSchema("TR_MASTER");
    //    }
    //    public ArriendoDataContext() : base(GetConnection(), true)
    //    {

    //    }

    //    public DbSet<Arriendo> ArriendosDbSet { get; set; }
    //    public DbSet<Servicios> ServiciosDbSet { get; set; }

    //    private static OracleConnection GetConnection()
    //    {
    //        return OracleBD.GetConnection();
    //    }
    //}
    //clase acompañante

    public class Acompañante
    {
        [Required]
        public int Id_Acompannante { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apeliido { get; set; }
    }

    #region ServiciosClass
    public class Servicios
    {

        public int? Id_servicio { get; set; }
        public string Nombre_servicio { get; set; }
        public string Descripcion { get; set; }
        public string Disponible { get; set; }
        public int? Count_disponibilidad { get; set; }
        public int? Costo_servicio { get; set; }
        public List<Servicios> ServiciosDisponibles { get; set; }


        public void RegistrarServicio(Servicios servicios)
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
        }

        public Servicios ObtenerServicioPorId(int? id)
        {
            Servicios servicio = null;

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SELECT * FROM SERVICIO WHERE id_servicio = :id", connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            servicio = new Servicios
                            {
                                Id_servicio = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("id_servicio"))),
                                Nombre_servicio = Convert.ToString(reader.GetValue(reader.GetOrdinal("nombre_servicio"))),
                                Descripcion = Convert.ToString(reader.GetValue(reader.GetOrdinal("descripcion"))),
                                Disponible = Convert.ToString(reader.GetValue(reader.GetOrdinal("disponible"))),
                                Count_disponibilidad = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("count_disponibilidad"))),
                                Costo_servicio = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("costo_servicio")))
                            };
                        }
                        return servicio;
                    }
                }
            }
        }

        public List<Servicios> ObtenerNombreServicio(Servicios serv)
        {
            List<Servicios> serviciosObtenidos = new List<Servicios>();

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();


                using (OracleCommand command = new OracleCommand("SELECT id_servicio, nombre_servicio FROM SERVICIO", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Servicios servicio = new Servicios
                            {
                                Id_servicio = Convert.ToInt32(reader["id_servicio"]),
                                Nombre_servicio = reader["nombre_servicio"].ToString(),
                            };

                            serviciosObtenidos.Add(servicio);
                        }

                    }
                }
            }
            serv.ServiciosDisponibles = serviciosObtenidos;
            return serviciosObtenidos;
        }

        public Tuple<int, string, int> ObtenerNombreServicioTuple(Servicios serv)
        {
            // Existing code...
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("SELECT id_servicio, nombre_servicio, Costo_servicio FROM SERVICIO", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id_servicio = Convert.ToInt32(reader["id_servicio"]);
                            string nombre_servicio = reader["nombre_servicio"].ToString();
                            int costo_servicio = Convert.ToInt32(reader["Costo_servicio"]);

                            return Tuple.Create(id_servicio, nombre_servicio, costo_servicio);
                        }
                    }
                }
            }
            return null;
            // Existing code...
        }


        public List<Servicios> ObtenerTodosLosServicios()
        {
            List<Servicios> servicios = new List<Servicios>();

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SELECT * FROM SERVICIO", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Servicios servicio = new Servicios
                            {
                                Id_servicio = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("id_servicio"))),
                                Nombre_servicio = Convert.ToString(reader.GetValue(reader.GetOrdinal("nombre_servicio"))),
                                Descripcion = Convert.ToString(reader.GetValue(reader.GetOrdinal("descripcion"))),
                                Disponible = Convert.ToString(reader.GetValue(reader.GetOrdinal("disponible"))),
                                Count_disponibilidad = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("count_disponibilidad"))),
                                Costo_servicio = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("costo_servicio")))
                            };

                            servicios.Add(servicio);
                        }
                    }
                }
            }

            return servicios;
        }



    }
    #endregion

    public class CheckIn
    {
        public int Id_CheckIn { get; set; }
        public DateTime Fecha_Entrada { get; set; }
        public int Numero_Habitacion{ get; set; }
        public int Cantidad_huesped{ get; set; }
        public string Tiempo_estancia { get; set; }
        public string Metodo_pago { get; set; }
        public string Informacion_estadia { get; set; }
        public int Id_arriendo { get; set; }
        public int Id_invent { get; set; }
        public int Id_srv { get; set; }
    }

    public class CheckOut
    {
        public int Id_CheckOut { get;set;}
        public int Id_multa  { get;set;}//Si no tiene multa no se puede acceder a cambios en el campo el valor va ser null si corresponde
        public string Tipo_Devolucion  { get;set;}
        public int Id_Depto_Out  { get; set;}
        public int Id_mant  { get; set;}
        public byte[] Firma  { get; set;}
    }

    public class DetalleMulta
    {
        public int Id_Multa { get; set; }
        public int Valor_Multa { get; set; }
        public string Detalle_Multa { get; set; }
    }

    public class Inventario
    {
        public int Id_Inventario { get; set; }
        public string Stock { get; set; }
        public Byte[] ImagenStock { get; set; }
        public string Descripcion { get; set; }
        public int NumItem { get; set; }
        public int NumDepto { get; set; }
    }

    public class Mantencion //Ver informacion adicional que puede ir en esta tabla
    {
        public int Id_Mantencion { get; set; }
        public string Descripcion { get; set; }
        public int Id_Mant {get; set;}//Revisar esta columna
    }

    public class Reserva
    {
        public int Id_Reserva { get; set; }
        public int Id_Usuario { get; set; }
        public int Id_Acompannante { get; set; }
        public int Id_Depto { get; set; }
        public int Id_Registro { get; set; }
        public int Id_Reg_Servicio { get; set; }
        public int Id_CheckIn{ get; set; }
        //Modificar en base de datos y agregar check out
    }

    public class ArriendoDataAccess
    {
        public char ObtenerDisponibilidad(int idDepto)
        {
            char disponibilidad;

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("SELECT disponibilidad FROM DEPTO WHERE id_depto = :p_id_depto", connection))
                {
                    command.Parameters.Add(new OracleParameter("p_id_depto", idDepto));
                    disponibilidad = Convert.ToChar(command.ExecuteScalar());
                }
            }

            return disponibilidad;
        }
        public void RealizarArriendo(DateTime fechaIni, DateTime fechaTermi,
                                     int valorDia, int totalPago, string metodoPago, string estadoPago,
                                     int idUsers, int idDeptos,int idServicios, int idAcompannante)
        {
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("PKG_ARRIENDO.SN_ARRIENDO", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_fecha_ini", OracleDbType.Date).Value = fechaIni;
                    command.Parameters.Add("p_fecha_termi", OracleDbType.Date).Value = fechaTermi;
                    command.Parameters.Add("p_valor_dia", OracleDbType.Int32).Value = valorDia;
                    command.Parameters.Add("p_total_pago", OracleDbType.Int32).Value = totalPago;
                    command.Parameters.Add("p_metodo_pago", OracleDbType.Varchar2).Value = metodoPago;
                    command.Parameters.Add("p_estado_pago", OracleDbType.Varchar2).Value = estadoPago;
                    command.Parameters.Add("p_id_users", OracleDbType.Int32).Value = idUsers;
                    command.Parameters.Add("p_id_deptos", OracleDbType.Int32).Value = idDeptos;
                    command.Parameters.Add("p_id_servicios", OracleDbType.Int32).Value = idServicios;
                    command.Parameters.Add("p_id_acompannante", OracleDbType.Int32).Value = idAcompannante;
                    command.ExecuteNonQuery();
                }
            }
        }


    }
}
