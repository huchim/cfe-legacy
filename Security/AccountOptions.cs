using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Cfe.Security
{
    public class AccountOptions
    {
       
        public AccountOptions()
        {
            this.ModuleRepository = this.GetConfig("ModuleRepository", "Module");
            this.UserPrivilegeRepository = this.GetConfig("UserPrivilegeRepository", "UserPrivilege");
            this.PrivilegeRepository = this.GetConfig("PrivilegeRepository", "Privilege");
            this.RoleRepository = this.GetConfig("RoleRepository", "Role");
            this.UsersRepository = this.GetConfig("UsersRepository", "User");
        }

        /// <summary>
        /// Obtiene la configuración de usuarios desde el archivo de configuración de la aplicación.
        /// </summary>
        /// <param name="option">Nombre de la opción a buscar.</param>
        /// <param name="value">Valor predeterminado en caso de no existir en el archivo de configuración de la aplicación.</param>
        /// <returns>Cadena con el valor establecido.</returns>
        private string GetConfig(string option, string value)
        {
            return (string)(ConfigurationManager.AppSettings["Account::" + option] ?? value);
        }

        /// <summary>
        /// Nombre de la tabla de módulos.
        /// </summary>
        public string ModuleRepository { get; set; }

        public string UserPrivilegeRepository { get; set; }
        public string PrivilegeRepository { get; set; }
        public string RoleRepository { get; set; }
        public string UsersRepository { get; set; }
    }
}
