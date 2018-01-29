using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ITipologieClientiRepository 
    {
        IEnumerable<TipologiaCliente> GetAll();

        Task<TipologiaCliente> GetOneAsync(int idTipologia);
    }
}
