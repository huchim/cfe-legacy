namespace Cfe.Security
{
    using Cfe.Web.Db;

    public class ManagerObject
    {

        public ManagerObject()
        {
            this.Table = "Modules";
        }

        public Util.IModelBase Add(Util.IModelBase obj)
        {
            if (obj.Id != 0)
            {
                throw new InsertException("Object already has Id value");
            }

            return SqlClient.Insert(obj, this.Table);
        }

        public Util.IModelBase Find(int id)
        {
            // Genero la consulta.
            string sql = string.Format("{0} WHERE Id = {1} AND Status <> -1", SqlClient.CreateQuerySelect(this.ActiveObject, this.Table), id);

            this.ActiveObject = SqlClient.Select(this.ActiveObject, sql);

            return this.ActiveObject;
        }

        public string Table { get; set; }
        public Util.IModelBase ActiveObject { get; set; }
    }
}
