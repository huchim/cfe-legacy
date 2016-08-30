﻿namespace Cfe.Security
{
    using System;

    /// <summary>
    /// Organiza los permisos de tal manera que esten ligados a un módulo en especial.
    /// </summary>
    public class Location : Models.ModelBase, ILocation
    {
        /// <summary>
        /// Obtiene o establece el nombre del módulo.
        /// </summary>
        /// <value>Nombre del módulo.</value>
        public string Name { get; set; }
    }
}