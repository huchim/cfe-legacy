namespace Cfe.Security
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Organiza los permisos de tal manera que esten ligados a un módulo en especial.
    /// </summary>
    public class Role : Models.ModelBase, IRole
    {
        /// <summary>
        /// Obtiene o establece el nombre del módulo.
        /// </summary>
        /// <value>Nombre del módulo.</value>
        [Required, MinLength(4, ErrorMessage="El nombre del rol debe ser mayor a 3 caracteres"), MaxLength(30)]
        public string Name { get; set; }

        public void Include(IUser User)
        {
            // Tengo que ligar al usuario al rol.
            UserPrivilege up = new UserPrivilege() { UserId = ((Util.IModelBase)User).Id, RoleId = this.Id };

            // Crear el administrador de privilegios.
            UserPrivilegeManager upm = new UserPrivilegeManager();

            // Y crear el rol.
            upm.Add(up);
        }
    }
}