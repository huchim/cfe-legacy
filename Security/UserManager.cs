
namespace Cfe.Security
{
        public class UserManager : ManagerObject
        {
            public UserManager()
            {
                this.Table = "Users";
                this.ActiveObject = new User();
            }

            public IUser Find(string name)
            {
                // Genero la consulta de búsqueda.
                string sql = string.Format("SELECT TOP (1) Id FROM {0} WHERE UserName = '{1}' AND Status <> -1", this.Table, name);

                // Solicito el identificador.
                int objId = Cfe.Data.SqlClient.ExecuteScalar(sql);

                // Si existe
                if (objId > -1)
                {
                    // Si existe.
                    return this.Find(objId);
                }

                return null;

            }

            public IUser Find(int id)
            {
                return (IUser)base.Find(id);
            }
        }

    }
