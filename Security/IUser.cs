﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfe.Security
{
    public interface IUser
    {
        string Name { get; set; }

        string UserName { get; set; }

        string Password { get; set; }

        string Email { get; set; }


    }
}
