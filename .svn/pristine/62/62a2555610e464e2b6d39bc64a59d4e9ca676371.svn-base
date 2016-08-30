namespace Cfe.Security
{
    public class ModuleManager : ManagerObject
    {
        public ModuleManager()
        {
            this.Table = "Modules";
            this.ActiveObject = new Module();
        }

        public IModule Find(string name, bool createIfNotExists)
        {
            // Genero la consulta de búsqueda.
            string sql = string.Format("SELECT TOP (1) Id FROM {0} WHERE Name = '{1}' AND Status <> -1", this.Table, name);

            // Solicito el identificador.
            int objId = Data.SqlClient.ExecuteScalar(sql);

            // No existe y desea crearlo.
            if (objId == -1 && createIfNotExists)
            {
                // Reiniciar en caso de problemas.
                this.ActiveObject = new Module();

                ((Module)this.ActiveObject).Name = name;

                // Devuelve el objeto recien creado.
                return (IModule)this.Add(this.ActiveObject);

            } // NO existe y no desea crearlo.
            else if (objId > -1)
            {
                // Si existe.
                return this.Find(objId);
            }

            return null;

        }

        public IModule Find(int id)
        {
            return (IModule)base.Find(id);
        }
    }
}
