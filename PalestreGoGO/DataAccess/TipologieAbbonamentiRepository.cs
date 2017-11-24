using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public class TipologieAbbonamentiRepository : MultitenantEntityBaseRepository<TipologieAbbonamenti>, ITipologieAbbonamentiRepository
    {
        public TipologieAbbonamentiRepository(PalestreGoGoDbContext context) : base(context)
        {

        }
    }
}
