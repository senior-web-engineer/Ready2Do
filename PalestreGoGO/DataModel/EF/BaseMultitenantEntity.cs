using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public abstract class BaseMultitenantEntity: BaseEntity
    {
        public int IdCliente { get; set; }

    }
}
