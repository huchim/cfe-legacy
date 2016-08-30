﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfe.Security
{
    public class RoleManager : ManagerObject
    {
        public RoleManager()
        {
            this.Table = "Roles";
            this.ActiveObject = new Role();
        }


        public IRole Find(string name, bool createIfNotExists)
        {
            // Genero la consulta de búsqueda.
            string sql = string.Format("SELECT TOP (1) Id FROM {0} WHERE Name = '{1}' AND Status <> -1", this.Table, name);

            // Solicito el identificador.
            int objId = Cfe.Data.SqlClient.ExecuteScalar(sql);

            // No existe y desea crearlo.
            if (objId == -1 && createIfNotExists)
            {
                // Reiniciar en caso de problemas.
                this.ActiveObject = new Role();

                ((Role)this.ActiveObject).Name = name;

                // Devuelve el objeto recien creado.
                return (IRole)this.Add(this.ActiveObject);

            } // NO existe y no desea crearlo.
            else if (objId > -1)
            {
                // Si existe.
                return this.Find(objId);
            }

            return null;

        }

        public IRole Find(int id)
        {
            return (IRole)base.Find(id);
        }
    }
}
