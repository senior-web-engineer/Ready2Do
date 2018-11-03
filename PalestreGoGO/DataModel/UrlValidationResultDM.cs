using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public enum UrlValidationResultDM : int
    {
        OK = 1,
        Reserved = -1,
        AlreadyUsed = -2
    }
}
