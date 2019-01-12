namespace ready2do.model.common
{
    public class LocationInputDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public short? CapienzaMax { get; set; }
        public string Colore { get; set; }
        public string UrlImage { get; set; }
        public string UrlIcon { get; set; }
    }
}
