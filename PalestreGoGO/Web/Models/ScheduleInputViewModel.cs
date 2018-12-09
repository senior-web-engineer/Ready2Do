using ready2do.model.common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ScheduleInputViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Nome evento")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Tipo Lezione")]
        public int? IdTipoLezione { get; set; }

        [Required]
        [Display(Name ="Sala")]
        public int? IdLocation { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name ="Data Evento")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Data { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Ora Evento")]
        public TimeSpan? OraInizio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Apertura Iscrizioni")]
        public DateTime? DataAperturaIscrizioni { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ora Apertura Iscrizioni")]
        public TimeSpan? OraAperturaIscrizioni { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Chiusura Iscrizioni")]
        public DateTime? DataChiusuraIscrizioni { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ora Chiusura Iscrizioni")]
        public TimeSpan? OraChiusuraIscrizioni { get; set; }

        [Display(Name = "Istruttore")]
        [MaxLength(150)]
        public string Istruttore { get; set; }

        [Required]
        [Display(Name = "Posti Disponibili")]
        public int PostiDisponibili { get; set; }

        [Display(Name = "Cancellabile fino al giorno")]
        [DataType(DataType.Date)]

        public DateTime? DataCancellazioneMax { get; set; }

        [Display(Name = "Cancellabile fino all'ora")]
        [DataType(DataType.Time)]
        public TimeSpan? OraCancellazioneMax { get; set; }

        [MaxLength(100)]
        public string Note { get; set; }

        [Required]
        public bool CancellazioneConsentita { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Visibile dal giorno")]
        public DateTime? VisibileDalDate { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Visibile dalle ore")]
        public TimeSpan? VisibileDalTime { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Visibile fino al giorno")]
        public DateTime? VisibileFinoAlDate { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Visibile fino alle ore")]
        public TimeSpan? VisibileFinoAlTime { get; set; }

        [Required]
        public bool WaitListDisponibile { get; set; }

        public ScheduleRecurrencyDM Recurrency { get; set; }

        [Required]
        public ScheduleTypeDM TipoSchedule { get; set; }
    }
}
