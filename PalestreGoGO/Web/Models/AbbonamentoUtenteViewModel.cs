using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AbbonamentoUtenteViewModel : AbbonamentoUtenteInputModel
    {
        public string NomeTipoAbbonamento { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public DateTime? DataCreazione { get; set; }

        //public IEnumerable<KeyValuePair<int, string>> TipologieAbbonamento { get; set; }

        public AbbonamentoUtenteViewModel()
        {

        }

        public AbbonamentoUtenteViewModel(AbbonamentoUtenteInputModel item)
        {
            Id = item.Id;
            IdCliente = item.IdCliente;
            UserId = item.UserId;
            IdTipoAbbonamento = item.IdTipoAbbonamento;
            DataInizioValidita = item.DataInizioValidita;
            Scadenza = item.Scadenza;
            IngressiIniziali = item.IngressiIniziali;
            IngressiResidui = item.IngressiResidui;
            Importo = item.Importo;
            ImportoPagato = item.ImportoPagato;

        }
    }
}
