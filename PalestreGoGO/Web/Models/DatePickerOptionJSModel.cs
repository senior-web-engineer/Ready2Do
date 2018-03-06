using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class DatePickerOptionJSModel
    {
        public DatePickerOptionJSModel()
        {
            I18n = new DatePickerInternationalizationOptions();
            FirstDay = 1; //0 = Domenica, 1= Lunedì, ecc..
            SetDefaultDate = true;
            Format = "yyyy-mm-dd";
            ShowClearBtn = false;
        }
        public string Format { get; set; }
        public string DefaultDate { get; set; }
        public bool SetDefaultDate { get; set; }
        public int FirstDay { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }
        public bool ShowClearBtn { get; set; }
        public DatePickerInternationalizationOptions I18n { get; set; }
    }

    public class DatePickerInternationalizationOptions
    {
        public DatePickerInternationalizationOptions()
        {
            Months = new string[] { "Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno", "Luglio", "Agosto", "Settembre", "Ottobre", "Novembre", "Dicembre" };
            MonthsShort = new string[] { "Gen", "Feb", "Mar", "Apr", "Mag", "Giu", "Lug", "Ago", "Set", "Ott", "Nov", "Dic" };
            Weekdays = new string[] { "Domenica", "Lunedì", "Martedì", "Mercoledì", "Giovedì", "Venerdì", "Sabato" };
            WeekdaysShort = new string[] { "Dom", "Lun", "Mar", "Mer", "Gio", "Ven", "Sab", "Dom" };
            WeekdaysAbbrev = new string[] { "D", "L", "M", "M", "G", "V", "S" };
            Cancel = "Annulla";
            Done = "Ok";
            Clear = "Clear";
        }
        public string Cancel { get; set; }
        public string Clear { get; set; }
        public string Done { get; set; }
        public string[] Months { get; set; }
        public string[] MonthsShort { get; set; }
        public string[] Weekdays { get; set; }
        public string[] WeekdaysShort { get; set; }
        public string[] WeekdaysAbbrev { get; set; }
    }
}
