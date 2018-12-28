namespace ready2do.model.common
{
    public class OrarioAperturaDM
    {
        public OrarioAperturaDM()
        {
            Lunedi = new GiornoAperturaDM();
            Martedi = new GiornoAperturaDM();
            Mercoledi = new GiornoAperturaDM();
            Giovedi = new GiornoAperturaDM();
            Venerdi = new GiornoAperturaDM();
            Sabato = new GiornoAperturaDM();
            Domenica = new GiornoAperturaDM();
        }

        public GiornoAperturaDM Lunedi { get; set; }
        public GiornoAperturaDM Martedi { get; set; }
        public GiornoAperturaDM Mercoledi { get; set; }
        public GiornoAperturaDM Giovedi { get; set; }
        public GiornoAperturaDM Venerdi { get; set; }
        public GiornoAperturaDM Sabato { get; set; }
        public GiornoAperturaDM Domenica { get; set; }

    }
}
