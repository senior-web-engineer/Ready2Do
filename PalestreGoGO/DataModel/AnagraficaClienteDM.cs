namespace PalestreGoGo.DataModel
{
    public class AnagraficaClienteDM
    {
        public int IdCliente { get; set; }

        public string Nome { get; set; }

        public string RagioneSociale { get; set; }

        public string Email { get; set; }

        public string NumTelefono { get; set; }

        public string Descrizione { get; set; }

        public string UrlRoute { get; set; }

        public string Indirizzo { get; set; }

        public string Citta { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public float? Latitudine { get; set; }
        public float? Longitudine { get; set; }
    }

}
