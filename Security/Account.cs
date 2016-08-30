using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfe.Security
{
    public static class Account
    {
        /// <summary>
        /// Acceso a la sesión en el servdior.
        /// </summary>
        private static Session SessionObject = new Session();

        /// <summary>
        /// Cadena de consulta
        /// </summary>
        private static AccountOptions AccountOptions = new AccountOptions();

        private static string GetEntriesSql()
        {
            return "SELECT DISTINCT [" + AccountOptions.ModuleRepository + 
                   "].[Name] AS Module, [" + AccountOptions.PrivilegeRepository + "].[Name] AS Privilege FROM [" + 
                   AccountOptions.UserPrivilegeRepository + "] INNER JOIN [" + AccountOptions.RoleRepository + 
                   "] ON [" + AccountOptions.UserPrivilegeRepository + "].[RoleId] = [" + AccountOptions.RoleRepository + 
                   "].[Id] INNER JOIN [" + AccountOptions.PrivilegeRepository + "] ON [" + AccountOptions.RoleRepository + 
                   "].[Id] = [" + AccountOptions.PrivilegeRepository + "].[RoleId] INNER JOIN [" + 
                   AccountOptions.ModuleRepository + "] ON [" + AccountOptions.PrivilegeRepository + "].[ModuleId] = [" + 
                   AccountOptions.ModuleRepository + "].[Id] WHERE [" + AccountOptions.UserPrivilegeRepository + 
                   "].[UserId] = {0} AND ([" + AccountOptions.UserPrivilegeRepository + "].[Status] <> -1 AND [" + 
                   AccountOptions.RoleRepository + "].[Status] <> -1 AND [" + AccountOptions.ModuleRepository + 
                   "].[Status] <> -1 AND [" + AccountOptions.PrivilegeRepository + "].[Status] <> -1)"; 
        }

        /// <summary>
        /// Genera la sesión para el usuario identificado.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        public static void AutoLogin(int userId)
        {
            // Procedo a establecer los datos de la sesión.
            Session secureSession = new Session();

            // Aseguro la sesión para posteriores validaciones.
            secureSession.Secure(userId);

            // Genero la lista de permisos asignados a este usuario.
            SetupPrivileges(userId);
        }
        
        public static bool Login(string userName, string password)
        {
            // Generar la consulta SQL para obtener el identificador del usuario.
            string sqlLogin = string.Format("SELECT TOP(1) Id FROM [{0}] WHERE UserName = '{1}' AND Password = '{2}' AND Status <> -1", 
                                            AccountOptions.UsersRepository, 
                                            userName, 
                                            password);

            // Ejecutar la consulta.
            int id = Cfe.Data.SqlClient.ExecuteScalar(sqlLogin);

            if (id > 0)
            {
                // Procedo a establecer los datos de la sesión.
                AutoLogin(id);

                // Devuelvo el resultado correcto.
                return true;
            }

            // No hubo coincidencias para la consulta del usuario.
            return false;
        }

        public static bool IsLogged()
        {
            // Procedo a obtener la sesión segura.
            Session secureSession = new Session();

            if (secureSession.Open())
            {
                // Si la sesión se pudo abrir correctamente procedo a evaluar el Id.
                return (int)(System.Web.HttpContext.Current.Session["Id"] ?? 0) > 0;
            }
            else
            {
                // Devuelvo falso si la sesión es inválida.
                return false;
            }
        }

        /// <summary>
        /// Determina si el usuario actual tiene permisos para cierta acción.
        /// </summary>
        /// <param name="privilegeName">Nombre del privilegio.</param>
        /// <param name="moduleName">Nombre dl módulo.</param>
        /// <returns>Verdadero si tiene permisos.</returns>
        public static bool Can(string privilegeName, string moduleName)
        {
            if (!IsLogged())
            {
                return false;
            }

            if (IsAdmin())
            {
                return true;
            }

            return GetPrivileges().Any(x => x.Module == moduleName && x.Privilege == privilegeName);
        }

        public static bool IsAdmin()
        {
            if (!IsLogged())
            {
                return false;
            }

            return GetPrivileges().Any(x => x.Module == "System" && x.Privilege == "Admin");
        }

        public static void Logout()
        {
            Session securedSession = new Session();

            securedSession.Remove();
        }

        /// <summary>
        /// Devuelve la lista de permisos existentes en la sesión.
        /// </summary>
        /// <returns>Aségurese de usar Session.Open() antes de leer estos datos.</returns>
        private static List<AccessEntry> GetPrivileges()
        {
            return System.Web.HttpContext.Current.Session["Entries"] as List<AccessEntry>;
        }

        private static void SetupPrivileges(int id)
        {
            // Genero la consulta para conseguir la lista de permisos asignados a este usuario.
            string entriesSql = string.Format(GetEntriesSql(), id);

            // Ejecuto la consulta.
            System.Data.DataTable dtPrivileges = Data.SqlClient.ExecuteQuery(entriesSql);

            // Creo la lista de privilegios y lo asigno a la sesión.
            System.Web.HttpContext.Current.Session["Entries"] = CreatePrivilegesList(dtPrivileges).ToList();


        }

        private static IEnumerable<AccessEntry> CreatePrivilegesList(System.Data.DataTable dt)
        {
            // Enumero todos las filas de la tabla.
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                // Enlisto el permiso al resultado total.
                yield return new AccessEntry()
                {
                    Module = dr["Module"].ToString(),
                    Privilege = dr["Privilege"].ToString()
                };
            }
        }
    }
}
