using System;
using System.ComponentModel.DataAnnotations;

namespace ready2do.model.common
{
    public class NotificaDM
    {
        public long? Id { get; set; }
        public string UserId{ get; set; }
        public int IdTipo { get; set; }
        public int? IdCliente { get; set; }
        [MaxLength(50)]
        public string Titolo { get; set; }
        [MaxLength(250)]
        public string Testo { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime? DataDismissione { get; set; }
        public DateTime? DataInizioVisibilita { get; set; }
        public DateTime? DataFineVisibilita { get; set; }       
        public DateTime? DataPrimaVisualizzazione { get; set; }
    }
}
