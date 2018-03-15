using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public enum UserType
    {
        Anonymous   = 0,
        NormalUser  =0x1,
        Admin       =0x10,
        Owner       =0x100,
        GlobalAdmin = 0x1000000
    }
}
