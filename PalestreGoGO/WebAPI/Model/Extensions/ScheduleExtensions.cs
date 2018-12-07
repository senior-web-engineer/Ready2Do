using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Model.Extensions
{
    public static class ScheduleExtensions
    {
        /// <summary>
        /// Il metodo ritorna True solo per gli Schedule attualmente visibili pubblicamente, ovver:
        /// - Non cancellati
        /// - Con la data VisibileDal (se valorizzata) < NOW
        /// - altrimenti con
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static bool IsPublicVisible (this ScheduleBaseDM schedule)
        {
            //Escludiamo i cancellati
            if (schedule.DataCancellazione.HasValue) return false;
            //Escludiamo quelli non ancora visibili
            if (schedule.VisibileDal.HasValue && schedule.VisibileDal.Value > DateTime.Now) return false;
            //Escludiamo quelli non più visibili
            if (schedule.VisibileFinoAl.HasValue && schedule.VisibileFinoAl.Value < DateTime.Now) return false;
            return true;
        }
    }
}
