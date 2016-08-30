namespace Cfe.Security
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Organiza los permisos de tal manera que esten ligados a un módulo en especial.
    /// </summary>
    public class Module : Models.ModelBase, IModule
    {
        /// <summary>
        /// Obtiene o establece el nombre del módulo.
        /// </summary>
        /// <value>Nombre del módulo.</value>
        [Required, MinLength(4, ErrorMessage = "El nombre del módulo debe ser mayor a 3 caracteres"), MaxLength(30)]
        public string Name { get; set; }
    }
}