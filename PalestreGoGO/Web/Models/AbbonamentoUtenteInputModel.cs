using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AbbonamentoUtenteInputModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Selezionare una tipologia di abbonamento")]
        public int? IdTipoAbbonamento { get; set; }
        [Required(ErrorMessage = "Inserire la data di inizio validità dell'abbonamento")]
        public DateTime? DataInizioValidita { get; set; }
        [Required(ErrorMessage = "Inserire la data di scadenza dell'abbonamento")]
        public DateTime? Scadenza { get; set; }
        public short? IngressiIniziali { get; set; }
        public short? IngressiResidui { get; set; }
        public decimal? Importo { get; set; }
        public decimal? ImportoPagato { get; set; }

        /// <summary>
        /// Se valorizzato a True indica che gli appuntamenti in sospeso (da confermare) devono essere
        /// confermati utilizzando questo Abbonamento
        /// </summary>
        /// <remarks>
        /// Non possiamo usare un nullable perché Razor non supporta il binding a questo tipo di dati in un checkbox (2-state control)
        /// </remarks>
        public bool ConfermaAppuntamenti { get; set; }

        public string ReturnUrl { get; set; }
    }
}
