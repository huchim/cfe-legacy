﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfe.Security
{
    using System.ComponentModel.DataAnnotations;

    public class User : Models.ModelBase, IUser
    {

        [MaxLength(40)]
        public string Name { get; set; }

        [Required, DataType(DataType.Password)]
        public string UserName { get; set; }
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="La contraseña debe ser de más de al menos 8 caracteres"), MaxLength(64)]
        public string Password
        {
            get;
            set;
        }
    }
}
