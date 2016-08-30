﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Cfe.Data
{
    public static class SqlClient
    {
        private static string SConnectionStringName = "AppContext";

        public static SqlConnection DbConnection(string ConnectionStringName)
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ToString());
        }

        public static void SetupConnectionStringName(string ConnectionStringName)
        {
            SConnectionStringName = ConnectionStringName;
        }

        public static SqlConnection DbConnection()
        {
            return DbConnection(SConnectionStringName);
        }

        /// <summary>
        /// Ejecuta una consulta de actualización en la base de datos definida.
        /// </summary>
        /// <param name="SqlQuery">Consulta de actualización.</param>
        public static void ExecuteNonQuery(string SqlQuery)
        {
            ExecuteNonQuery(SqlQuery, SConnectionStringName);
        }

        /// <summary>
        /// Ejecuta una consulta de actualización en la base de datos definida.
        /// </summary>
        /// <param name="SqlQuery">Consulta de actualización.</param>
        /// <param name="ConnectionStringName">Cadena de conexión a la base de datos.</param>
        public static void ExecuteNonQuery(string SqlQuery, string ConnectionStringName)
        {
            ExecuteNonQuery(SqlQuery, new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ToString()));
        }

        /// <summary>
        /// Ejecuta una consulta de actualización en la base de datos definida.
        /// </summary>
        /// <param name="SqlQuery">Consulta de actualización.</param>
        /// <param name="DbConnection"></param>
        public static void ExecuteNonQuery(string SqlQuery, SqlConnection DbConnection)
        {
            using (DbConnection)
            {
                // COneción establecida			
                SqlCommand DbCommand = new SqlCommand(SqlQuery, DbConnection);
                DbConnection.Open();
                DbCommand.ExecuteNonQuery();
            }
        }

        public static SqlConnection GetSqlConnection()
        {
            ConnectionStringSettings connectionSettings;

            connectionSettings = ConfigurationManager.ConnectionStrings[SConnectionStringName];

            if (connectionSettings == null)
            {
                throw new Exception("No se pudo localizar la cadena de conexión: " + SConnectionStringName);
            }

            return new SqlConnection(connectionSettings.ConnectionString);
        }

        public static int ExecuteScalar(string SqlQuery)
        {

            SqlConnection DbConnection = GetSqlConnection();

            using (DbConnection)
            {
                // COneción establecida			
                SqlCommand DbCommand = new SqlCommand(SqlQuery, DbConnection);
                DbConnection.Open();

                // En caso de error.
                int r = -1;

                try
                {
                    // Ejecuto y verifico el resultado.
                    r = (int)DbCommand.ExecuteScalar();
                }
                catch (Exception)
                {
                    // En caso de error.
                    r = -1;
                }

                // Devuelvo el resultado.
                return r;
            }
        }

        /// <summary>
        /// Ejecuta una consulta a la base de datos.
        /// </summary>
        /// <param name="SqlQuery">Consulta SQL.</param>
        /// <returns>Tabla con los resultados.</returns>
        public static DataTable ExecuteQuery(string SqlQuery)
        {
            return ExecuteQuery(SqlQuery, SConnectionStringName);
        }

        /// <summary>
        /// Ejecuta una consulta a la base de datos.
        /// </summary>
        /// <param name="SqlQuery">Consulta SQL.</param>
        /// <param name="ConnectionStringName">Nombre de la conexión a la base de datos definida en WebConfig</param>
        /// <returns>Tabla con los resultados.</returns>
        public static DataTable ExecuteQuery(string SqlQuery, string ConnectionStringName)
        {
            return ExecuteQuery(SqlQuery, new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ToString()));
        }

        /// <summary>
        /// Ejecuta una consulta a la base de datos.
        /// </summary>
        /// <param name="SqlQuery">Consulta SQL.</param>
        /// <param name="DbConnection">Conexión a la base de datos.</param>
        /// <returns>Tabla con los resultados.</returns>
        public static DataTable ExecuteQuery(string SqlQuery, SqlConnection DbConnection)
        {
            DataTable t = new DataTable();

            using (DbConnection)
            {
                // Abro la conexión
                DbConnection.Open();

                // Ejecuto la consulta
                using (SqlDataAdapter a = new SqlDataAdapter(SqlQuery, DbConnection))
                {
                    a.Fill(t);
                    return t;
                }
            }
        }
    }
}