/* Internazionalizzazione */
function MaterilizeI18N() {
    var datePickerI18n = {
        "it": {
            months: ['Gennaio', 'Febbraio', 'Marzo', 'Aprile', 'Maggio', 'Giugno', 'Luglio', 'Agosto', 'Settembre', 'Ottobre', 'Novembre', 'Dicembre'],
            monthsShort: ['Gen', 'Feb', 'Mar', 'Apr', 'Mag', 'Giu', 'Lug', 'Ago', 'Set', 'Ott', 'Nov', 'Dic'],
            weekdays: ['Domenica', 'Lunedì', 'Martedì', 'Mercoledì', 'Giovedì', 'Venerdì', 'Sabato'],
            weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mer', 'Gio', 'Ven', 'Sab', 'Dom'],
            weekdaysAbbrev: ['D', 'L', 'M', 'M', 'G', 'V', 'S'],
            clear: 'Cancella',
            cancel: 'Annulla',
            today: 'Oggi'
        }
    };

    var timePickeri18n = {
        "it":
        {
            cancel: 'Annulla',
            clear: 'Cancella',
            done: 'Ok'
        }
    };

    this.getDateI18N = function (local) {
        //Per ora gestiamo solo l'italiano
        return datePickerI18n["it"];
    }
    this.getTimeI18N = function (local) {
        //Per ora gestiamo solo l'italiano
        return timePickeri18n["it"];
    }
};

export default new MaterilizeI18N()
//const r2sMatI18N = 