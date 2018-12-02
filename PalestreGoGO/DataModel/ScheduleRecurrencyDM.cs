using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public abstract class ScheduleRecurrencyDM
    {
        [JsonProperty(PropertyName = "Recurrency")]
        public string Recurrency { get; private set; }

        [JsonProperty(PropertyName = "DaysOfWeek", NullValueHandling =NullValueHandling.Ignore)]
        public List<string> DaysOfWeek { get; private set; }

        public ScheduleRecurrencyDM(RepeatScheduleEveryDM recurrency)
        {
            this.Recurrency = recurrency.ToString();
        }

        public ScheduleRecurrencyDM(GiorniSettimanaDM giorni)
        {
            this.Recurrency = RepeatScheduleEveryDM.Weekly.ToString();
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

    public enum RepeatScheduleEveryDM
    {
        Daily,
        Weekly,
        Monthly
    }

    [Flags]
    public enum GiorniSettimanaDM
    {
        Nessuno = 0,
        Lunedi = 1 << 1,
        Martedi = 1 << 2,
        Mercoledi = 1 << 3,
        Giovedi = 1 << 4,
        Venerdi = 1 << 5,
        Sabato = 1 << 6,
        Domenica = 1 << 7
    }
}
