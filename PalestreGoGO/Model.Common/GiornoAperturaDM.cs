namespace ready2do.model.common
{
    public class GiornoAperturaDM
    {
        public GiornoAperturaDM()
        {
            Mattina = new FasciaOrariaAperturaDM();
            Pomeriggio = new FasciaOrariaAperturaDM();
            TipoOrario = TipoOrarioAperturaDM.Spezzato;
        }

        public FasciaOrariaAperturaDM Mattina { get; set; }
        public FasciaOrariaAperturaDM Pomeriggio { get; set; }
        public TipoOrarioAperturaDM TipoOrario { get; set; }

    }
}
