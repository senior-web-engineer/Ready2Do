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
        Scaduto = 20,
        IngressiTerminati = 30,
        AbbonamentoValido = 100
    }

    public class ClienteUtenteViewModel
    {
        public UserHeaderViewModel UserInfo { get; set; }
        //public int IdCliente { get; set; }
        //public string IdUtente { get; set; }
        //public string Nome { get; set; }
        //public string Cognome { get; set; }
        //public string DisplayName { get; set; }

        //public string Email { get; set; }
        //public string NumTelefono { get; set; }
        //public DateTime DataAssociazione { get; set; }
        //public ClienteUtenteStatoViewModel Stato { get; set; }

        //public StatoAbbonamento StatoAbbonamento
        //{
        //    get
        //    {
        //        if (Abbonamento == null) return StatoAbbonamento.NessunAbbonamento;
        //        if (Abbonamento.Scadenza < DateTime.Now) { return StatoAbbonamento.Scaduto; }
        //        if (Abbonamento.IngressiResidui.HasValue && (Abbonamento.IngressiResidui.Value <= 0)) { return StatoAbbonamento.IngressiTerminati; }
        //        return StatoAbbonamento.AbbonamentoValido;
        //    }
        //}

        //public AbbonamentoUtenteViewModel Abbonamento {get;set;}

        public IList<AbbonamentoUtenteViewModel> Abbonamenti { get; set; }

        public IList<CertificatUtenteViewModel> Certificati { get; set; }

        public IList<AppuntamentoUtenteViewModel> Appuntamenti { get; set; }
    }
}
