using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using PagedList;


namespace TursimoReal.Models
{

    public class Departamentos
    {
        public int Id_depto { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public int Numero_Depto { get; set; }
        public string Descripcion { get; set; }
        public byte[] Imagen { get; set; }
        public int Valor_Arriendo { get; set; }
        public string Comentario { get; set; }
        public int Calificacion { get; set; }
        public int Region { get; set; }
        public int Admin_Encargado { get; set; }
        public char Disponibilidad { get; set; } 

        public List<Departamentos> ListarDepartamentos { get; set; }

        #region MetodosDepartamento
        #region GuardarDeptos

        public void GuardarDepartamento(Departamentos departamento)
        {
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                OracleCommand cmd = new OracleCommand("SP_REGISTRAR_DEPARTAMENTOS", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = departamento.Nombre;
                cmd.Parameters.Add("p_direccion", OracleDbType.Varchar2).Value = departamento.Direccion;
                cmd.Parameters.Add("p_numero_depto", OracleDbType.Int32).Value = departamento.Numero_Depto;
                cmd.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = departamento.Descripcion;
                cmd.Parameters.Add("p_imagen", OracleDbType.Blob).Value = departamento.Imagen;
                cmd.Parameters.Add("p_valor_arriendo", OracleDbType.Int32).Value = departamento.Valor_Arriendo;
                cmd.Parameters.Add("p_comentario", OracleDbType.Varchar2).Value = departamento.Comentario;
                cmd.Parameters.Add("p_calificacion", OracleDbType.Int32).Value = departamento.Calificacion;
                cmd.Parameters.Add("p_region", OracleDbType.Int32).Value = departamento.Region;
                cmd.Parameters.Add("p_admin_encargado", OracleDbType.Int32).Value = departamento.Admin_Encargado;
                cmd.Parameters.Add("p_disponibilidad", OracleDbType.Char).Value = departamento.Disponibilidad;

                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region ObtenerDeptosID
        public Departamentos ObtenerDepartamentoPorId(int id)
        {
            Departamentos departamento = null;

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SELECT * FROM DEPTO WHERE id_depto = :id", connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departamento = new Departamentos
                            {
                                Id_depto = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("id_depto"))),
                                Nombre = Convert.ToString(reader.GetValue(reader.GetOrdinal("Nombre"))),
                                Direccion = Convert.ToString(reader.GetValue(reader.GetOrdinal("direccion"))),
                                Numero_Depto = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("numero_depto"))),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal("descripcion"))),
                                Imagen = (byte[])reader.GetValue(reader.GetOrdinal("imagen")),
                                Valor_Arriendo = (int)Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("valor_arriendo"))),
                                Comentario = reader.IsDBNull(reader.GetOrdinal("comentario")) ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal("comentario"))),
                                Calificacion = (int)(reader.IsDBNull(reader.GetOrdinal("calificacion")) ? (int?)null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("calificacion")))),
                                Region = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("region"))),
                                Admin_Encargado = (int)(reader.IsDBNull(reader.GetOrdinal("Admin_Encargado")) ? (int?)null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Admin_Encargado")))),
                                Disponibilidad = Convert.ToChar(reader.GetValue(reader.GetOrdinal("disponibilidad")))
                            };
                        };
                        return departamento;
                    }
                }
            }
        }
        #endregion

        #region ObtenerImagen
        public FileContentResult ObtenerImagen(int id)
        {
            byte[] imagenBytes;

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                string query = "SELECT imagen FROM depto WHERE id_depto = :id";
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":id", id));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            imagenBytes = (byte[])reader["imagen"];

                            string contentType = "image/png";
                            return new FileContentResult(imagenBytes, contentType);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        #endregion

        #region PaginarWeb
        public IPagedList<Departamentos> PaginadoDepartamentos(int? page)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;

            List<Departamentos> departamentos = new List<Departamentos>();

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM Depto ORDER BY Id_depto";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Departamentos departamento = new Departamentos()
                            {
                                Id_depto = Convert.ToInt32(reader["Id_depto"]),
                                Nombre = reader["Nombre"].ToString(),
                                Direccion = reader["Direccion"].ToString(),
                                Numero_Depto = Convert.ToInt32(reader["Numero_Depto"]),
                                Descripcion = reader["Descripcion"].ToString(),
                                Imagen = (byte[])reader["Imagen"],
                                Valor_Arriendo = Convert.ToInt32(reader["Valor_Arriendo"]),
                                Comentario = reader["Comentario"].ToString(),
                                Calificacion = Convert.ToInt32(reader["Calificacion"]),
                                Region = Convert.ToInt32(reader["Region"]),
                                Admin_Encargado = Convert.ToInt32(reader["Admin_Encargado"]),
                                Disponibilidad = Convert.ToChar(reader["Disponibilidad"])
                            };

                            departamentos.Add(departamento);
                        }
                    }
                }
            }

            var pagedDepartamentos = new StaticPagedList<Departamentos>(
                departamentos.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                pageNumber, pageSize, departamentos.Count);

            return pagedDepartamentos;
        }
        #endregion

        #region ModificarDepto
        public void ModificarDepartamento(Departamentos departamento)
        {
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "SP_ACTUALIZAR_DEPARTAMENTOS";


                    command.Parameters.Add("p_id_depto", OracleDbType.Int32, ParameterDirection.Input).Value = departamento.Id_depto;
                    command.Parameters.Add("p_nombre", OracleDbType.Varchar2, ParameterDirection.Input).Value = departamento.Nombre;
                    command.Parameters.Add("p_direccion", OracleDbType.Varchar2, ParameterDirection.Input).Value = departamento.Direccion;
                    command.Parameters.Add("p_numero_depto", OracleDbType.Int32, ParameterDirection.Input).Value = departamento.Numero_Depto;
                    command.Parameters.Add("p_descripcion", OracleDbType.Varchar2, ParameterDirection.Input).Value = departamento.Descripcion;
                    command.Parameters.Add("p_imagen", OracleDbType.Blob, ParameterDirection.Input).Value = departamento.Imagen;
                    command.Parameters.Add("p_valor_arriendo", OracleDbType.Decimal, ParameterDirection.Input).Value = departamento.Valor_Arriendo;
                    command.Parameters.Add("p_comentario", OracleDbType.Varchar2, ParameterDirection.Input).Value = departamento.Comentario;
                    command.Parameters.Add("p_calificacion", OracleDbType.Int32, ParameterDirection.Input).Value = departamento.Calificacion;
                    command.Parameters.Add("p_region", OracleDbType.Int32, ParameterDirection.Input).Value = departamento.Region;
                    command.Parameters.Add("p_admin_encargado", OracleDbType.Int32, ParameterDirection.Input).Value = departamento.Admin_Encargado;
                    command.Parameters.Add("p_disponibilidad", OracleDbType.Char, ParameterDirection.Input).Value = departamento.Disponibilidad;

                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region ObtenerTodosLosDepto
        public List<Departamentos> ObtenerTodosLosDepartamentos()
        {
            List<Departamentos> listaDepartamentos = new List<Departamentos>();

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SELECT * FROM DEPTO", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Departamentos departamento = new Departamentos
                            {
                                Id_depto = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("id_depto"))),
                                Nombre = Convert.ToString(reader.GetValue(reader.GetOrdinal("Nombre"))),
                                Direccion = Convert.ToString(reader.GetValue(reader.GetOrdinal("direccion"))),
                                Numero_Depto = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("numero_depto"))),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal("descripcion"))),
                                Imagen = (byte[])reader.GetValue(reader.GetOrdinal("imagen")),
                                Valor_Arriendo = (int)Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("valor_arriendo"))),
                                Comentario = reader.IsDBNull(reader.GetOrdinal("comentario")) ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal("comentario"))),
                                Calificacion = (int)(reader.IsDBNull(reader.GetOrdinal("calificacion")) ? (int?)null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("calificacion")))),
                                Region = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("region"))),
                                Admin_Encargado = (int)(reader.IsDBNull(reader.GetOrdinal("Admin_Encargado")) ? (int?)null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Admin_Encargado")))),
                                Disponibilidad = Convert.ToChar(reader.GetValue(reader.GetOrdinal("disponibilidad")))
                            };

                            listaDepartamentos.Add(departamento);
                        }
                    }
                }
            }

            return listaDepartamentos;
        }
        #endregion

        #region ObtenerTodosLosDepartamentosDiponibles
        public List<Departamentos> ObtenerTodosLosDepartamentosDiponibles()
        {
            List<Departamentos> listaDepartamentos = new List<Departamentos>();

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SELECT * FROM DEPTO WHERE disponibilidad = 'S'", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Departamentos departamento = new Departamentos
                            {
                                Id_depto = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("id_depto"))),
                                Nombre = Convert.ToString(reader.GetValue(reader.GetOrdinal("Nombre"))),
                                Direccion = Convert.ToString(reader.GetValue(reader.GetOrdinal("direccion"))),
                                Numero_Depto = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("numero_depto"))),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal("descripcion"))),
                                Imagen = (byte[])reader.GetValue(reader.GetOrdinal("imagen")),
                                Valor_Arriendo = (int)Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("valor_arriendo"))),
                                Comentario = reader.IsDBNull(reader.GetOrdinal("comentario")) ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal("comentario"))),
                                Calificacion = (int)(reader.IsDBNull(reader.GetOrdinal("calificacion")) ? (int?)null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("calificacion")))),
                                Region = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("region"))),
                                Admin_Encargado = (int)(reader.IsDBNull(reader.GetOrdinal("Admin_Encargado")) ? (int?)null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Admin_Encargado")))),
                                Disponibilidad = Convert.ToChar(reader.GetValue(reader.GetOrdinal("disponibilidad")))
                            };

                            listaDepartamentos.Add(departamento);
                        }
                    }
                }
            }

            return listaDepartamentos;
        }

        #endregion

        #region ObtenerListaDepartamentosPorID

        public List<Departamentos> ObtenerListaDepartamentosPorId(int id)
        {
            ArriendoDataAccess arriendoDataAccess = new ArriendoDataAccess();
            List<Departamentos> departamentos = new List<Departamentos>();

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SELECT * FROM DEPTO WHERE id_depto = :id", connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Departamentos departamento = new Departamentos
                            {
                                Id_depto = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("id_depto"))),
                                Nombre = Convert.ToString(reader.GetValue(reader.GetOrdinal("Nombre"))),
                                Direccion = Convert.ToString(reader.GetValue(reader.GetOrdinal("direccion"))),
                                Numero_Depto = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("numero_depto"))),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal("descripcion"))),
                                Imagen = (byte[])reader.GetValue(reader.GetOrdinal("imagen")),
                                Valor_Arriendo = (int)Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("valor_arriendo"))),
                                Comentario = reader.IsDBNull(reader.GetOrdinal("comentario")) ? null : Convert.ToString(reader.GetValue(reader.GetOrdinal("comentario"))),
                                Calificacion = (int)(reader.IsDBNull(reader.GetOrdinal("calificacion")) ? (int?)null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("calificacion")))),
                                Region = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("region"))),
                                Admin_Encargado = (int)(reader.IsDBNull(reader.GetOrdinal("Admin_Encargado")) ? (int?)null : Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Admin_Encargado")))),
                                Disponibilidad = Convert.ToChar(reader.GetValue(reader.GetOrdinal("disponibilidad")))
                            };

                            // Obtener la disponibilidad del departamento
                            departamento.Disponibilidad = arriendoDataAccess.ObtenerDisponibilidad(departamento.Id_depto);


                            departamentos.Add(departamento);
                        }
                    }
                }
            }

            return departamentos;
        }
        #endregion 
        #endregion

                public void GuardarIdRegion(int idRegion)
        {
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("UPDATE DEPTO SET region = :idRegion", connection))
                {
                    command.Parameters.Add(new OracleParameter("idRegion", idRegion));
                    command.ExecuteNonQuery();
                }
            }
        }
    }





    public class Regiones
    {
        public int Id_region { get; set; }
        public string NombreRegion { get; set; }
        public List<Regiones> regionesDisponibles { get; set; }



        public List<Regiones> ObtenerNombreRegion(Regiones reg)
        {
            List<Regiones> RegionesObtenidos = new List<Regiones>();

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();


                using (OracleCommand command = new OracleCommand("SELECT Region FROM REGION", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Regiones regiones = new Regiones
                            {
                                Id_region = Convert.ToInt32(reader["Id_region"]),
                                NombreRegion = reader["Region"].ToString()
                            };

                            RegionesObtenidos.Add(regiones);
                        }

                    }
                }
            }
            reg.regionesDisponibles = RegionesObtenidos;
            return RegionesObtenidos;
        }


        public Regiones ObtenerRegionesPorId(int id)
        {
            Regiones region = null;

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SELECT id_region FROM REGION WHERE id_region = :id", connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            region = new Regiones
                            {
                                Id_region = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("id_region"))),

                            };
                        };
                        return region;
                    }
                }
            }
        }
        public List<Regiones> ObtenerRegiones()
        {
            Regiones region = new Regiones();
            List<Regiones> RegionesObtenidos = new List<Regiones>();
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SELECT * FROM REGION", connection))
                {

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            region = new Regiones
                            {
                                Id_region = Convert.ToInt32(reader["Id_region"]),
                                NombreRegion = reader["Region"].ToString()

                            };
                            RegionesObtenidos.Add(region);
                        };
                        return RegionesObtenidos;
                    }
                }
            }
        }




    }
}





