﻿using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public interface ITipologieClientiRepository 
    {
        IEnumerable<TipologieClienti> GetAll();
    }
}
