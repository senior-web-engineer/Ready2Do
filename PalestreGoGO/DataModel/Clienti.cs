using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class Clienti : BaseEntity
    {
        public Clienti()
        {
            AbbonamentiUtenti = new HashSet<AbbonamentiUtenti>();
            ClientiImmagini = new HashSet<ClientiImmagini>();
            ClientiMetadati = new HashSet<ClientiMetadati>();
            Locations = new HashSet<Locations>();
            Schedules = new HashSet<Schedules>();
            TipologieAbbonamenti = new HashSet<TipologieAbbonamenti>();
            TipologieLezioni = new HashSet<TipologieLezioni>();
            UrlRoute = "";
        }

        [Required]
        [StringLength(250)]
        public string Nome { get; set; }
        [Required]
        [StringLength(150)]
        public string RagioneSociale { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(50)]
        public string NumTelefono { get; set; }
        [StringLength(1000)]
        public string Descrizione { get; set; }
        public int IdTipologia { get; set; }
        [Required]
        [StringLength(250)]
        public string Indirizzo { get; set; }
        [Required]
        [StringLength(100)]
        public string Citta { get; set; }
        [Required]
        [StringLength(10)]
        public string ZipOrPostalCode { get; set; }
        [Required]
        [StringLength(100)]
        public string Country { get; set; }
        public double? Latitudine { get; set; }
        public double? Longitudine { get; set; }
        [Column(TypeName = "datetime2(2)")]
        public DateTime DataCreazione { get; set; }
        public Guid IdUserOwner { get; set; }
        [Required]
        [StringLength(500)]
        public string ProvisioningToken { get; set; }
        [Column(TypeName = "datetime2(2)")]
        public DateTime? DataProvisioning { get; set; }       
        [Required]
        [StringLength(100)]
        public string UrlRoute { get; set; }        
        public string OrarioApertura { get; set; }

        [ForeignKey("IdTipologia")]
        [InverseProperty("Clienti")]
        public TipologiaCliente IdTipologiaNavigation { get; set; }
        [InverseProperty("IdClienteNavigation")]
        public ICollection<AbbonamentiUtenti> AbbonamentiUtenti { get; set; }
        [InverseProperty("IdClienteNavigation")]
        public ICollection<ClientiImmagini> ClientiImmagini { get; set; }
        [InverseProperty("IdClienteNavigation")]
        public ICollection<ClientiMetadati> ClientiMetadati { get; set; }
        [InverseProperty("IdClienteNavigation")]
        public ICollection<ClientiUtenti> ClientiUtenti { get; set; }
        [InverseProperty("IdClienteNavigation")]
        public ICollection<Locations> Locations { get; set; }
        [InverseProperty("IdClienteNavigation")]
        public ICollection<Schedules> Schedules { get; set; }
        [InverseProperty("IdClienteNavigation")]
        public ICollection<TipologieAbbonamenti> TipologieAbbonamenti { get; set; }
        [InverseProperty("IdClienteNavigation")]
        public ICollection<TipologieLezioni> TipologieLezioni { get; set; }
        [InverseProperty("Cliente")]
        public ICollection<Appuntamenti> Appuntamenti { get; set; }

    }
}
