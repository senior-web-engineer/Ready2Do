using System;
using System.Collections.Generic;

namespace ready2do.model.common
{
    public class ScheduleInputDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string Title { get; set; }
        public int IdTipoLezione { get; set; }
        public int IdLocation { get; set; }
        public DateTime DataOraInizio { get; set; }
        public string Istruttore { get; set; }
        public int PostiDisponibili { get; set; }
        public int? PostiResidui { get; set; }
        public bool CancellazioneConsentita { get; set; }
        public DateTime? CancellabileFinoAl { get; set; }
        public DateTime? DataAperturaIscrizione { get; set; }
        public DateTime? DataChiusuraIscrizione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public DateTime? VisibileDal { get; set; }
        public DateTime? VisibileFinoAl { get; set; }
        public bool WaitListDisponibile { get; set; }
        public string UserIdOwner { get; set; }
        public string Note { get; set; }
        public ScheduleRecurrencyDM Recurrency { get; set; }
        public int? IdParent { get; set; }
        public ScheduleTypeDM TipoSchedule { get; set; }

    }

}
