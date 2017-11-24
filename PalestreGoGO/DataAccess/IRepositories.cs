using System;
using System.Collections.Generic;
using System.Text;
using PalestreGoGo.DataModel;

namespace PalestreGoGo.DataAccess
{
    public interface ITipologieAbbonamentiRepository : IMultitenantEntityRepository<int,TipologieAbbonamenti, int> { }
    public interface ITipologieLezioniRepository : IMultitenantEntityRepository<int, TipologieLezioni, int> { }


    //public interface ITipologieClientiRepository : IEntityBaseRepository<TipologieClienti> { }

    //public interface ITipologieImmaginiRepository : IEntityBaseRepository<TipologieImmagini> { }
}
