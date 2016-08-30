using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cfe.Security
{
    using System.ComponentModel.DataAnnotations;

    public class Privilege : Models.ModelBase, IPrivilege
    {
        /// <summary>
        /// Obtiene o establece el nombre del privilegio.
        /// </summary>
        /// <value>Nombre del módulo.</value>
        [Required, MinLength(2, ErrorMessage = "El nombre del módulo debe ser mayor a 1 caracteres"), MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o establece la relación del permiso con un módulo.
        /// </summary>
        /// <value>Módulo asociado.</value>
        public int ModuleId { get; set; }


        public int RoleId { get; set; }

    }
}
