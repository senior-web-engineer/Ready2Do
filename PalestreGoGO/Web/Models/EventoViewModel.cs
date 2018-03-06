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
        public int IdCliente { get; set; }

        public int? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public int? IdTipoLezione { get; set; }

        public int? IdLocation { get; set; }
        [Required]
        public DateTime? Data { get; set; }

        [Required]
        public TimeSpan? OraInizio { get; set; }

        public string Istruttore { get; set; }

        [Required]
        public int PostiDisponibili { get; set; }

        [Required]
        public DateTime? CancellabileFinoAl { get; set; }

        public string Note { get; set; }

    }

    public class EventoViewModel : EventoInputViewModel
    {
        public EventoViewModel()
        {
            DataEventoOptions = new DatePickerOptionJSModel()
            {
                DefaultDate = DateTime.Now.ToString("yyyy-MM-dd"),
                MinDate = DateTime.Now.ToString("yyyy-MM-dd"),
                SetDefaultDate = true
            };
            DataCancellazioneOptions = new DatePickerOptionJSModel()
            {
                DefaultDate = DateTime.Now.ToString("yyyy-MM-dd"),
                MinDate = DateTime.Now.ToString("yyyy-MM-dd"),
                SetDefaultDate = true
            };
        }
        public IEnumerable<SelectListItem> TipologieLezioni { get; set; }

        public DatePickerOptionJSModel DataEventoOptions { get; set; }
        public DatePickerOptionJSModel DataCancellazioneOptions { get; set; }
    }
}
