﻿namespace Cfe.Web.Db
{
    using System;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;

    public static class SqlClient
    {
        public static Util.IModelBase Select(Util.IModelBase obj, string query)
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = obj.GetType().GetProperties();

            // Genero la consulta.
            DataTable dt = Cfe.Data.SqlClient.ExecuteQuery(query);

            // No se encontraron resultados.
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            // Aunque es una mala práctica escribir en el código el índice del registro, debemos asumir que si es mayor a 0
            // al menos 1 registro habra sido obtenido, lo cual me permitirá poner el índice a 0 (basado en 0)

            // Recorro las propiedades del objeto.
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                // Obtengo el valor de la base de datos
                object val = dt.Columns.Contains(propertyInfo.Name) ? (dt.Rows[0][propertyInfo.Name] == DBNull.Value ? null : dt.Rows[0][propertyInfo.Name]) : null;

                // Establezco el valor al objeto.
                propertyInfo.SetValue(obj, val);
            }

            return obj;
        }

        public static string CreateQuerySelect(Util.IModelBase obj, string tableName)
        {
            // Crear una variable para almacenar los valores a insertar.
            List<string> values = new List<string>();

            PropertyInfo[] propertyInfos;
            propertyInfos = obj.GetType().GetProperties();


            // Recorrer todas las propiedades con la finalidad de normalizar la cadena de insersión.
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                values.Add(propertyInfo.Name);
            }

            return string.Format("SELECT {0} FROM {1}", string.Join(",", values.ToArray()), tableName);

        }

        public static Util.IModelBase Insert(Util.IModelBase obj, string tableName)
        {
            // Crear una variable para almacenar los valores a insertar.
            Dictionary<string, string> values = new Dictionary<string, string>();

            PropertyInfo[] propertyInfos;
            propertyInfos = obj.GetType().GetProperties();

            // Recorrer todas las propiedades con la finalidad de normalizar la cadena de insersión.
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name != "Id")
                {

                    if (propertyInfo.PropertyType == typeof(DateTime))
                    {
                        // Se usa la conversión ODBC canónico (con milisegundos) para una insersión correcta.
                        object val = propertyInfo.GetValue(obj);
                        string vals = string.Format("convert(datetime,'{0}',21)", ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        values.Add(propertyInfo.Name, vals);
                    }

                    if (propertyInfo.PropertyType == typeof(bool))
                    {
                        object val = propertyInfo.GetValue(obj);
                        string vals = (bool)val ? "1" : "0";
                        values.Add(propertyInfo.Name, vals);
                    }

                    if (propertyInfo.PropertyType == typeof(int))
                    {
                        object val = propertyInfo.GetValue(obj);
                        string vals = ((int)val).ToString();
                        values.Add(propertyInfo.Name, vals);
                    }

                    if (propertyInfo.PropertyType == typeof(double))
                    {
                        object val = propertyInfo.GetValue(obj);
                        string vals = ((double)val).ToString();
                        values.Add(propertyInfo.Name, vals);
                    }

                    if (propertyInfo.PropertyType == typeof(float))
                    {
                        object val = propertyInfo.GetValue(obj);
                        string vals = ((float)val).ToString();
                        values.Add(propertyInfo.Name, vals);
                    }

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        object val = propertyInfo.GetValue(obj);
                        string vals = "'" + ((string)val).Replace("'", "''") + "'";
                        values.Add(propertyInfo.Name, vals);
                    }
                }
            }

            // Genero la consulta.
            string sql = string.Format("INSERT INTO {0} ({1}) OUTPUT Inserted.Id VALUES ({2});", tableName, string.Join(",", values.Keys), string.Join(",", values.Values));
            System.Diagnostics.Debug.Print(sql);
            // Realizo la insersión.
            obj.Id = Cfe.Data.SqlClient.ExecuteScalar(sql);

            return obj;
        }
    }
}