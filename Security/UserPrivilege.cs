namespace Cfe.Security
{
    using System;

    /// <summary>
    /// Organiza los permisos de tal manera que esten ligados a un módulo en especial.
    /// </summary>
    public class UserPrivilege : Models.ModelBase, IUserPrivilege
    {
        public virtual int UserId { get; set; }
        
        public int RoleId { get; set; }

        
        public virtual Role Role { get; set; }
    }
}