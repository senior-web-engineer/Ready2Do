using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TipologiaAbbonamentoViewModel
    {
        public int? Id { get; set; }

        public int IdCliente { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Nome troppo lungo (max. 100 caratteri)"]
        [Display(Name = "Nome abbonamento")]
        public string Nome { get; set; }

        [Display(Name = "Durata in Mesi")]
        public short? DurataMesi { get; set; }

        [Display(Name = "Numero di Ingressi")]
        public short? NumIngressi { get; set; }

        public decimal? Costo { get; set; }

        [Display(Name = "Livello massimo corsi")]
        public short? MaxLivCorsi { get; set; }
    }
}
