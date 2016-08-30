namespace Cfe.Security
{
    using System;
    using System.Linq;
    using System.Web;

    public class Manager
    {
        private bool IsInitialized = false;
        private Session SessionObject = new Session();

        public Manager()
        {
            System.Diagnostics.Debug.Print("Controlado de seguridad inicializado.");

            this.Locations = new LocationManager();
            this.Modules = new ModuleManager();
            this.Roles = new RoleManager();
            this.Users = new UserManager();
            this.Privileges = new PrivilegeManager();
            this.UserPrivilege = new UserPrivilegeManager();
            

            this.IntEntriesQuery = "SELECT DISTINCT " + this.Modules.Table + ".Name AS Module, [" + this.Privileges.Table + "].Name AS Privilege FROM " + this.UserPrivilege.Table + " INNER JOIN " + this.Roles.Table + " ON " + this.UserPrivilege.Table + ".RoleId = " + this.Roles.Table + ".Id INNER JOIN [" + this.Privileges.Table + "] ON " + this.Roles.Table + ".Id = [" + this.Privileges.Table + "].RoleId INNER JOIN " + this.Modules.Table + " ON [" + this.Privileges.Table + "].ModuleId = " + this.Modules.Table + ".Id WHERE " + this.UserPrivilege.Table + ".UserId = {0} AND ( " + this.UserPrivilege.Table + ".Status <> -1 AND " + this.Roles.Table + ".Status <> -1 AND " + this.Modules.Table + ".Status <> -1 AND [" + this.Privileges.Table + "].Status <> -1)";
                        
        }

        public void UpdateSession()
        {            
            // Asegura la sessión.
            this.SessionObject = new Session();

            // Inicializo el objeto.
            System.Collections.Generic.List<AccessEntry> bufferObjects = HttpContext.Current.Session["Entries"] as System.Collections.Generic.List<AccessEntry>;

        }
        
        public bool Login(string UserName, string Password)
        {
            string sqlLogin = string.Format("SELECT Id FROM {0} WHERE UserName = '{1}' AND Password = '{2}'", this.Users.Table, UserName, Password);

            // Ejecuto la consulta.
            System.Data.DataTable dt = Data.SqlClient.ExecuteQuery(sqlLogin);
            System.Diagnostics.Debug.Print("Consultando base de datos: {0}", sqlLogin);

            if (dt.Rows.Count != 1)
            {
                return false;
            }
            else
            {
                int id = dt.Rows[0]["Id"] == DBNull.Value ? 0 : (int)dt.Rows[0]["Id"];

                if (id == 0)
                {
                    System.Diagnostics.Debug.Print("Usuario no identificado");
                    return false;
                }
                else {
                    // Aseguro la sessión.
                    this.SessionObject.Secure(id);

                    // Crear sesión...
                    HttpContext.Current.Session["Id"] = id;

                    System.Diagnostics.Debug.Print("Solicitando permisos...");
                    this.GetEntries();

                    return true;
                }
            }
        }

        public int UserId { get; set; }

        public string EntriesQuery
        {
            set { this.IntEntriesQuery = value; }

        }

        public bool Logged { get { return this.UserId != 0; } }

        public LocationManager Locations { get; set; }

        public ModuleManager Modules { get; set; }

        public PrivilegeManager Privileges { get; set; }

        public RoleManager Roles { get; set; }

        public UserManager Users { get; set; }
        

        public UserPrivilegeManager UserPrivilege { get; set; }

        internal string IntEntriesQuery { get; set; }

        private System.Collections.Generic.List<AccessEntry> AccessEntries { get; set; }

        public ILocation AddLocation(ILocation locationObject)
        {
            return (ILocation)this.Locations.Add(locationObject);
        }

        public IModule AddModule(IModule moduleObject)
        {
            return (IModule)this.Modules.Add(moduleObject);
        }

        public IModule AddModule(IModule moduleObject, IRole roleObject, string[] privilegeList)
        {
            IModule moduleObjectResult;

            // En caso de que el módulo ya exista no lo agrego.
            if (moduleObject.Id == 0)
            {
                moduleObjectResult = this.AddModule(moduleObject);
            }
            else
            {
                moduleObjectResult = moduleObject;
            }

            foreach (string privilege in privilegeList)
            {
                this.AddPrivilege(new Privilege() { Name = privilege, ModuleId = moduleObjectResult.Id, RoleId = roleObject.Id } );
            }

            return moduleObjectResult;
        }

        public IPrivilege AddPrivilege(IPrivilege privilegeObject)
        {
            return (IPrivilege)this.Privileges.Add(privilegeObject);
        }

        public bool Can(string privilegeName, string moduleName)
        {
            if (this.SessionObject.Open())
            {
                // Inicializo el objeto en busca de la sesión.
                System.Collections.Generic.List<AccessEntry> bufferObjects = HttpContext.Current.Session["Entries"] as System.Collections.Generic.List<AccessEntry>;

                return bufferObjects.Any(x => x.Module == moduleName && x.Privilege == privilegeName);
            }

            return false;
        }

        public static bool Access(string privilegeName, string moduleName)
        {
            // Al ser un método estático, es necesario cargar siempre el objeto de permisos.
            // Inicializo el objeto.
            System.Collections.Generic.List<AccessEntry> bufferObjects = HttpContext.Current.Session["Entries"] as System.Collections.Generic.List<AccessEntry>;

            if (bufferObjects != null)
            {
                return bufferObjects.Any(x => x.Module == moduleName && x.Privilege == privilegeName);
            }

            return false;
        }

        private void GetEntries()
        {
            if (this.IsInitialized)
            {
                return;
            }
            else
            {
                this.IsInitialized = true;
            }

            System.Diagnostics.Debug.Print(string.Format(this.IntEntriesQuery, this.UserId));

            // Ejecuto la consulta.
            System.Data.DataTable dt = Data.SqlClient.ExecuteQuery(string.Format(this.IntEntriesQuery, this.UserId));

            // Revisar los resultados que devolvió la consulta.
            this.AccessEntries = new System.Collections.Generic.List<AccessEntry>();

            // En caso de que la consulta sea correcta, procedo a crear los resultados
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                System.Diagnostics.Debug.Print("Agregando: {0}.{1}", dr["Module"].ToString(), dr["Privilege"].ToString());
                this.AccessEntries.Add(new AccessEntry() {
                    Module = dr["Module"].ToString(),
                    Privilege = dr["Privilege"].ToString()
                });
            }

            // Almacenar en la sesión:
            HttpContext.Current.Session["Entries"] = this.AccessEntries;

        }
    }
}