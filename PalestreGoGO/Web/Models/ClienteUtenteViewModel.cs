using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public enum StatoAbbonamento
    {
        NessunAbbonamento = 0,
        //InScadenza        = 10,
        Scaduto           = 20,
        IngressiTerminati = 30,
        AbbonamentoValido = 100
    }

    public class ClienteUtenteViewModel
    {
        public int IdCliente { get; set; }
        public Guid IdUtente { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }

        public StatoAbbonamento StatoAbbonamento
        {
            get
            {
                if (Abbonamento == null) return StatoAbbonamento.NessunAbbonamento;
                if (Abbonamento.Scadenza < DateTime.Now) { return StatoAbbonamento.Scaduto; }
                if (Abbonamento.IngressiResidui.HasValue && (Abbonamento.IngressiResidui.Value <= 0)) { return StatoAbbonamento.IngressiTerminati; }
                return StatoAbbonamento.AbbonamentoValido;
            }
        }

        public AbbonamentoUtenteViewModel Abbonamento {get;set;}
    }
}
