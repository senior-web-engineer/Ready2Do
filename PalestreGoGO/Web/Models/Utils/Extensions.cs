using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ready2do.model.common;

namespace Web.Models.Utils
{
    public static class Extensions
    {
        public static void GetMinMax(this GiornoAperturaDM  giorno, out TimeSpan? min, out TimeSpan? max)
        {
            min = max = null;
            if ((giorno == null) || (giorno.TipoOrario == TipoOrarioAperturaDM.Chiuso)) return;
            switch (giorno.TipoOrario)
            {
                case TipoOrarioAperturaDM.Continuato:
                case TipoOrarioAperturaDM.Spezzato:
                    min = giorno.Mattina?.Inizio;
                    max = giorno.Pomeriggio.Fine;
                    break;
                case TipoOrarioAperturaDM.Mattina:
                    min = giorno.Mattina?.Inizio;
                    max = giorno.Mattina?.Fine;
                    break;
                case TipoOrarioAperturaDM.Pomeriggio:
                    min = giorno.Pomeriggio?.Inizio;
                    max = giorno.Pomeriggio?.Fine;
                    break;
            }
        }

        public static void GetMinMax(this OrarioAperturaViewModel orario, out TimeSpan? min, out TimeSpan? max)
        {
            min = max = null;
            TimeSpan? inizio = default(TimeSpan?), 
                      fine = default(TimeSpan?);
            if (orario == null) return;
            orario?.Domenica?.GetMinMax(out inizio, out fine);
            if (inizio.HasValue) { min = inizio; }
            if (fine.HasValue) { max = fine; }
            orario?.LunVen?.GetMinMax(out inizio, out fine);
            if ((inizio.HasValue) && (!min.HasValue || (min.Value > inizio.Value))){ min = inizio; }
            if ((fine.HasValue) && (!max.HasValue || (max.Value < fine.Value))) { max= fine; }
            orario?.Sabato?.GetMinMax(out inizio, out fine);
            if ((inizio.HasValue) && (!min.HasValue || (min.Value > inizio.Value))) { min = inizio; }
            if ((fine.HasValue) && (!max.HasValue || (max.Value < fine.Value))) { max = fine; }
        }

        public static string AsString(this GiornoAperturaDM giorno)
        {
            if (giorno == null) return null;
            switch (giorno.TipoOrario)
            {
                case TipoOrarioAperturaDM.Chiuso:
                    return "Chiuso";
                case TipoOrarioAperturaDM.Continuato:
                    return $"Dalle {giorno.Mattina?.Inizio?.ToString(@"hh\:mm") ?? "-"} alle {giorno.Pomeriggio?.Fine?.ToString(@"hh\:mm") ?? "-"}";
                case TipoOrarioAperturaDM.Mattina:
                    return $"Dalle {giorno.Mattina?.Inizio?.ToString(@"hh\:mm") ?? "-"} alle {giorno.Pomeriggio?.Fine?.ToString(@"hh\:mm") ?? "-"}";
                case TipoOrarioAperturaDM.Pomeriggio:
                    return $"Dalle {giorno.Pomeriggio?.Inizio?.ToString(@"hh\:mm") ?? "-"} alle {giorno.Pomeriggio?.Fine?.ToString(@"hh\:mm") ?? "-"}";
                case TipoOrarioAperturaDM.Spezzato:
                    return $"Dalle {giorno.Mattina?.Inizio?.ToString(@"hh\:mm") ?? "-"} alle {giorno.Mattina?.Fine?.ToString(@"hh\:mm") ?? "-"} e dalle {giorno.Pomeriggio?.Inizio?.ToString(@"hh\:mm") ?? "-"} alle {giorno.Pomeriggio?.Fine?.ToString(@"hh\:mm") ?? "-"}";
                default:
                    return "--";
            }
            
        }
    }
}
