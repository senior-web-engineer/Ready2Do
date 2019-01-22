using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ready2do.model.common
{
    public class ScheduleRecurrencyDM
    {
        [JsonProperty(PropertyName = "Recurrency")]
        public string Recurrency { get; set; }

        [JsonProperty(PropertyName = "DaysOfWeek", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> DaysOfWeek { get; set; }

        [JsonProperty(PropertyName = "RepeatUntil", NullValueHandling = NullValueHandling.Ignore)]
        public string RepeatUntil { get; set; }

        [JsonProperty(PropertyName = "RepeatFor", NullValueHandling = NullValueHandling.Ignore)]
        public int? RepeatFor { get; set; }

        public ScheduleRecurrencyDM()
        {            
        }

        public ScheduleRecurrencyDM(RepeatScheduleEveryDM recurrency, DateTime repeatUntil)
        {
            this.Recurrency = recurrency.ToString();
            this.RepeatUntil = repeatUntil.ToString("yyyy-MM-dd");
        }

        public ScheduleRecurrencyDM(RepeatScheduleEveryDM recurrency, int repeatFor)
        {
            this.Recurrency = recurrency.ToString();
            this.RepeatFor = repeatFor;
        }

        public ScheduleRecurrencyDM(GiorniSettimanaDM giorni, DateTime repeatUntil)
        {
            this.Recurrency = RepeatScheduleEveryDM.Weekly.ToString();
            this.RepeatUntil = repeatUntil.ToString("yyyy-MM-dd");
            DaysOfWeek = new List<string>();
            if ((giorni | GiorniSettimanaDM.Lunedi) == GiorniSettimanaDM.Lunedi) DaysOfWeek.Add("Lun");
            if ((giorni | GiorniSettimanaDM.Martedi) == GiorniSettimanaDM.Martedi) DaysOfWeek.Add("Mar");
            if ((giorni | GiorniSettimanaDM.Mercoledi) == GiorniSettimanaDM.Mercoledi) DaysOfWeek.Add("Mer");
            if ((giorni | GiorniSettimanaDM.Giovedi) == GiorniSettimanaDM.Giovedi) DaysOfWeek.Add("Gio");
            if ((giorni | GiorniSettimanaDM.Venerdi) == GiorniSettimanaDM.Venerdi) DaysOfWeek.Add("Ven");
            if ((giorni | GiorniSettimanaDM.Sabato) == GiorniSettimanaDM.Sabato) DaysOfWeek.Add("Sab");
            if ((giorni | GiorniSettimanaDM.Domenica) == GiorniSettimanaDM.Domenica) DaysOfWeek.Add("Dom");
        }

        public ScheduleRecurrencyDM(GiorniSettimanaDM giorni, int repeatFor)
        {
            this.Recurrency = RepeatScheduleEveryDM.Weekly.ToString();
            this.RepeatFor = repeatFor;
            DaysOfWeek = new List<string>();
            if ((giorni | GiorniSettimanaDM.Lunedi) == GiorniSettimanaDM.Lunedi) DaysOfWeek.Add("Lun");
            if ((giorni | GiorniSettimanaDM.Martedi) == GiorniSettimanaDM.Martedi) DaysOfWeek.Add("Mar");
            if ((giorni | GiorniSettimanaDM.Mercoledi) == GiorniSettimanaDM.Mercoledi) DaysOfWeek.Add("Mer");
            if ((giorni | GiorniSettimanaDM.Giovedi) == GiorniSettimanaDM.Giovedi) DaysOfWeek.Add("Gio");
            if ((giorni | GiorniSettimanaDM.Venerdi) == GiorniSettimanaDM.Venerdi) DaysOfWeek.Add("Ven");
            if ((giorni | GiorniSettimanaDM.Sabato) == GiorniSettimanaDM.Sabato) DaysOfWeek.Add("Sab");
            if ((giorni | GiorniSettimanaDM.Domenica) == GiorniSettimanaDM.Domenica) DaysOfWeek.Add("Dom");
        }
    }
}
