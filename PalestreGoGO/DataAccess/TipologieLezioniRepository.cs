using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public class TipologieLezioniRepository : MultitenantEntityBaseRepository<TipologieLezioni>, ITipologieLezioniRepository
    {
        public TipologieLezioniRepository(PalestreGoGoDbContext context) : base(context)
        {

        }
    }
}
