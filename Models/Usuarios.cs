using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace TursimoReal.Models
{
    public class Usuarios
    {
        public int Id_user { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "No se permiten símbolos")]
        public string Nombre { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "No se permiten símbolos")]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Ingrese una dirección de correo electrónico válida")]
        public string Correo { get; set; }

        [Required]
        public int Numero { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "El campo debe tener como máximo 8 caracteres")]
        public string Clave { get; set; }

        [Required]
        public Rol IdRol { get; set; }

        public List<Usuarios> ListarUsuario { get; set; }

        public void RegistrarUsuario(Usuarios model)
        {
            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand("SP_REGISTRAR_USUARIOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_nombre", OracleDbType.Varchar2).Value = model.Nombre;
                    command.Parameters.Add("p_apellido", OracleDbType.Varchar2).Value = model.Apellido;
                    command.Parameters.Add("p_correo", OracleDbType.Varchar2).Value = model.Correo;
                    command.Parameters.Add("p_numero", OracleDbType.Int32).Value = model.Numero;
                    command.Parameters.Add("p_clave", OracleDbType.Varchar2).Value = model.Clave;
                    command.Parameters.Add("p_idrol_user", OracleDbType.Int32).Value = model.IdRol;

                    command.ExecuteNonQuery();

                    using (OracleCommand commitCmd = new OracleCommand("COMMIT", connection))
                    {
                        commitCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static Usuarios BuscarUsuarios(string correo, string clave)
        {
            Usuarios usuario = null;

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();

                string query = "SELECT NOMBRE, APELLIDO, CORREO, NUMERO, CLAVE, IDROL_USER FROM usuario WHERE Correo = :pcorreo AND Clave = :pclave";

                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":pcorreo", correo);
                    cmd.Parameters.Add(":pclave", clave);
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuarios()
                            {
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Numero = Convert.ToInt32(reader["Numero"]),
                                Clave = reader["Clave"].ToString(),
                                IdRol = (Rol)reader.GetInt32(reader.GetOrdinal("IdRol_User"))
                            };
                        }
                    }
                }
            }

            return usuario;
        }

        public List<Usuarios> ObtenerNombreUsuarios(Usuarios user)
        {

            List<Usuarios> UsuariosObtenidos = new List<Usuarios>();

            using (OracleConnection connection = OracleBD.GetConnection())
            {
                connection.Open();


                using (OracleCommand command = new OracleCommand("SELECT Id_usuario, Nombre FROM USUARIO WHERE IdRol_User = 1", connection))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuarios usuarios = new Usuarios
                            {
                                Id_user = Convert.ToInt32(reader["Id_usuario"]),
                                Nombre = reader["Nombre"].ToString()
                            };

                            UsuariosObtenidos.Add(usuarios);
                        }

                    }
                }
            }
            user.ListarUsuario = UsuariosObtenidos;
            return UsuariosObtenidos;
        }

        public enum Rol
        {
            Administrador = 1,
            Funcionario = 2,
            Cliente = 3
        }


    }
}