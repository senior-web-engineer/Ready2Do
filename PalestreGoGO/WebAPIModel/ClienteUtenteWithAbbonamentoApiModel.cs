using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    [Obsolete("Da eliminare")]
    public class ClienteUtenteWithAbbonamentoApiModel: ClienteUtenteApiModel
    {
        
        public AbbonamentoUtenteApiModel Abbonamento { get; set; }        
    }
}
