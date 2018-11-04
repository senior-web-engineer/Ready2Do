using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Utils
{
    public static class Extensions
    {
        public static void GetMinMax(this GiornoViewModel giorno, out TimeSpan? min, out TimeSpan? max)
        {
            min = max = null;
            if ((giorno == null) || (giorno.TipoOrario == TipoOrarioViewModel.Chiuso)) return;
            switch (giorno.TipoOrario)
            {
                case TipoOrarioViewModel.Continuato:
                case TipoOrarioViewModel.Spezzato:
                    min = TimeSpan.Parse(giorno.Mattina.Inizio);
                    max = TimeSpan.Parse(giorno.Pomeriggio.Fine);
                    break;
                case TipoOrarioViewModel.Mattina:
                    min = TimeSpan.Parse(giorno.Mattina.Inizio);
                    max = TimeSpan.Parse(giorno.Mattina.Fine);
                    break;
                case TipoOrarioViewModel.Pomeriggio:
                    min = TimeSpan.Parse(giorno.Pomeriggio.Inizio);
                    max = TimeSpan.Parse(giorno.Pomeriggio.Fine);
                    break;
            }
        }

        public static void GetMinMax(this OrarioAperturaViewModel orario, out TimeSpan? min, out TimeSpan? max)
        {
            min = max = null;
            TimeSpan? inizio, fine;
            if (orario == null) return;
            orario.Domenica.GetMinMax(out inizio, out fine);
            if (inizio.HasValue) { min = inizio; }
            if (fine.HasValue) { max = fine; }
            orario.LunVen.GetMinMax(out inizio, out fine);
            if ((inizio.HasValue) && (!min.HasValue || (min.Value > inizio.Value))){ min = inizio; }
            if ((fine.HasValue) && (!max.HasValue || (max.Value < fine.Value))) { max= fine; }
            orario.Sabato.GetMinMax(out inizio, out fine);
            if ((inizio.HasValue) && (!min.HasValue || (min.Value > inizio.Value))) { min = inizio; }
            if ((fine.HasValue) && (!max.HasValue || (max.Value < fine.Value))) { max = fine; }
        }

    }
}
