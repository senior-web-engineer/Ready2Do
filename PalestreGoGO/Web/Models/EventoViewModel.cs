using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    
    public class EventoInputViewModel
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


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data Chiusura Iscrizioni")]
        public DateTime? DataChiusuraIscrizioni { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Ora Chiusura Iscrizioni")]
        public TimeSpan? OraChiusuraIscrizioni { get; set; }

        [Display(Name = "Istruttore")]
        [MaxLength(150)]
        public string Istruttore { get; set; }

        [Required]
        [Display(Name = "Posti Disponibili")]
        public int PostiDisponibili { get; set; }

        [Required]
        [Display(Name = "Cancellabile fino al giorno")]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataCancellazioneMax { get; set; }

        [Required]
        [Display(Name = "Cancellabile fino all'ora")]
        [DataType(DataType.Time)]
        public TimeSpan? OraCancellazioneMax { get; set; }

        [MaxLength(100)]
        public string Note { get; set; }

    }

    public class EventoViewModel : EventoInputViewModel
    {
        public EventoViewModel()
        {
            DataEventoOptions = new DatePickerOptionJSModel()
            {
                DefaultDate = DateTime.Now,
                MinDate = DateTime.Now,
                SetDefaultDate = true
            };
            DataCancellazioneOptions = new DatePickerOptionJSModel()
            {
                DefaultDate = DateTime.Now,
                MinDate = DateTime.Now,
                SetDefaultDate = true
            };
        }

        public DatePickerOptionJSModel DataEventoOptions { get; set; }
        public DatePickerOptionJSModel DataCancellazioneOptions { get; set; }
    }
}
