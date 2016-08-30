﻿namespace Cfe.Security
{
    public interface IPrivilege : Util.IModelBase
    {
        /// <summary>
        /// Obtiene o establece el nombre del privilegio.
        /// </summary>
        /// <value>Nombre del módulo.</value>
        string Name { get; set; }

        /// <summary>
        /// Obtiene o establece la relación del permiso con un módulo.
        /// </summary>
        /// <value>Módulo asociado.</value>
        int ModuleId { get; set; }

        int RoleId { get; set; }
    }
}