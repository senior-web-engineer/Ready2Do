import r2doi18n from './r2do.i18n';


function _r2dEditEvento() {
    this._selectInstances = [];
    this._datePickDataEvento = null;
    this._timePickOraEvento = null;

    this._datePickCancellabileFinoAl = null;
    this._timePickCancellabileFinoAl = null;

    this._datePickAperturaIscriz = null;
    this._timePickAperturaIscriz = null;

    this._datePickChiusuraIscriz = null;
    this._timePickChiusuraIscriz = null;

    this._datePickInizioVisibIscriz = null;
    this._timePickInizioVisibIscriz = null;

    this._datePickFineVisibIscriz = null;
    this._timePickFineVisibIscriz = null;

    this._checkCancellazionConsentita = null;
    this._checkWaitListAvailable = null;

    this._dataEvento = null;

    this._tipoRicorrenza = null;
    /* OPZIONI */
    this._buildOpzioniDatePicker = function (minDate, defaultDate) {
        let result =
        {
            format: "dd/mm/yyyy",
            i18n: r2doi18n.getDateI18N()
        };
        if (defaultDate) {
            result.defaultDate = dataEvento;
            result.setDefaultDate = true;
        }
        if (minDate) {
            result.minDate = minDate;
        }
        return result;
    };
    this._builOpzioniTimePicker = function () {
        let result = {
            twelveHour: false,
            i18n: r2doi18n.getTimeI18N()
        };
        return result;
    };

    this._getElemById = function (elemId) {
        var elem = document.getElementById(elemId);
        return elem;
    };

    this.init = function (dataEvento, oraEvento) {
        console.log('EditEvento Init in progress...');
        let object = this;
        this._dataEvento = dataEvento;

        //Iniziliziamo tutte le select
        var selectElems = document.querySelectorAll('select');
        this._selectInstances = M.FormSelect.init(selectElems);

        //Iniziliziamo gli Expandables
        var accordionElems = document.querySelectorAll('.collapsible.expandable');
        M.Collapsible.init(accordionElems, { accordion: false });

        //Iniziliaziamo tutti i DatePicker e TimePicker
        this._datePickDataEvento = M.Datepicker.init(this._getElemById('dataEvento'), this._buildOpzioniDatePicker(new Date(), dataEvento));
        this._datePickCancellabileFinoAl = M.Datepicker.init(this._getElemById('dataCancellazione'), this._buildOpzioniDatePicker(new Date()));
        this._datePickAperturaIscriz = M.Datepicker.init(this._getElemById('dataAperturaIscrizioni'), this._buildOpzioniDatePicker(new Date()));
        this._datePickChiusuraIscriz = M.Datepicker.init(this._getElemById('dataChiusuraIscrizioni'), this._buildOpzioniDatePicker(new Date()));
        this._datePickInizioVisibIscriz = M.Datepicker.init(this._getElemById('visibileDalDate'), this._buildOpzioniDatePicker(new Date()));
        this._datePickFineVisibIscriz = M.Datepicker.init(this._getElemById('visibileFinoAlDate'), this._buildOpzioniDatePicker(new Date()));

        this._timePickOraEvento = M.Timepicker.init(this._getElemById('oraEvento'), this._builOpzioniTimePicker());
        this._timePickCancellabileFinoAl = M.Timepicker.init(this._getElemById('oraCancellazione'), this._builOpzioniTimePicker());
        this._timePickAperturaIscriz = M.Timepicker.init(this._getElemById('oraAperturaIscrizioni'), this._builOpzioniTimePicker());
        this._timePickChiusuraIscriz = M.Timepicker.init(this._getElemById('oraChiusuraIscrizioni'), this._builOpzioniTimePicker());
        this._timePickInizioVisibIscriz = M.Timepicker.init(this._getElemById('visibileDalTime'), this._builOpzioniTimePicker());
        this._timePickFineVisibIscriz = M.Timepicker.init(this._getElemById('VisibileFinoAlTime'), this._builOpzioniTimePicker());

        //CheckBox
        this._checkWaitListAvailable = this._getElemById('waitListDisponibile');
        this._checkCancellazionConsentita = this._getElemById('cancellazioneConsentita');
        this._checkCancellazionConsentita.addEventListener('change', function (e) { object._handleCancellazioneChange(e); });
        this._handleCancellazioneChange();//Lo chiamiamo esplicitamente per gestire lo stato iniziale

        //Ricorrenza
        this._tipoRicorrenza = this._getElemById('tipoRicorrenza');
        this._tipoRicorrenza.addEventListener('change', function (e) { object._handleTipoRicorrenzaChange(e); });

        //RadiButton Tipologia Fine Ricorrenza
        this._getElemById('rbFineRicorrenzaNum').addEventListener('change', function (e) {object._handleTipoFineRicorrenzaChange(e, 'rbFineRicorrenzaNum') });
        this._getElemById('rbFineRicorrenzaDate').addEventListener('change', function (e) { object._handleTipoFineRicorrenzaChange(e, 'rbFineRicorrenzaDate') });

        return 0;
    };

    this._handleCancellazioneChange = function (event) {
        if (this._checkCancellazionConsentita.checked) {
            this._getElemById('dataCancellazione').disabled = false;
            this._getElemById('oraCancellazione').disabled = false;
            this._checkWaitListAvailable.disabled = false;
        } else {
            this._getElemById('dataCancellazione').disabled = true;
            this._getElemById('oraCancellazione').disabled = true;
            this._checkWaitListAvailable.disabled = true;
        }
    };

    this._handleTipoRicorrenzaChange = function (event) {
        let value = this._tipoRicorrenza.value;
        console.log("Inside _handleTipoRicorrenzaChange. Value = " + value);
        switch (value) {
            case "none":
                console.log("handling value: " + value);
                this._manageSelectStatus(this._getElemById('weekDaysRecurrency'), false);
                this._getElemById('dataFineRecurrency').enabled = false;
                this._getElemById('numRipetizioni').enabled = false;
                this._getElemById('rbFineRicorrenzaNum').enabled = false;
                this._getElemById('rbFineRicorrenzaDate').enabled = false;
                break;
            case "daily":
                console.log("handling value: " + value);
                this._manageSelectStatus(this._getElemById('weekDaysRecurrency'), false);
                this._getElemById('dataFineRecurrency').enabled = true;
                this._getElemById('numRipetizioni').enabled = true;
                this._getElemById('rbFineRicorrenzaNum').enabled = true;
                this._getElemById('rbFineRicorrenzaDate').enabled = true;
                break;
            case "weekly":
                console.log("handling value: " + value);
                this._manageSelectStatus(this._getElemById('weekDaysRecurrency'), true);
                this._getElemById('dataFineRecurrency').enabled = true;
                this._getElemById('numRipetizioni').enabled = true;
                this._getElemById('rbFineRicorrenzaNum').enabled = true;
                this._getElemById('rbFineRicorrenzaDate').enabled = true;
                break;
            case "montly":
                console.log("handling value: " + value);
                this._manageSelectStatus(this._getElemById('weekDaysRecurrency'), true);
                this._getElemById('dataFineRecurrency').enabled = true;
                this._getElemById('numRipetizioni').enabled = true;
                this._getElemById('rbFineRicorrenzaNum').enabled = true;
                this._getElemById('rbFineRicorrenzaDate').enabled = true;
                break;
            default:
                console.log("handling ELSE value: " + value);
                break;
        }

        //if (this._checkCancellazionConsentita.checked) {
        //    this._getElemById('dataCancellazione').disabled = false;
        //    this._getElemById('oraCancellazione').disabled = false;
        //    this._checkWaitListAvailable.disabled = false;
        //} else {
        //    this._getElemById('dataCancellazione').disabled = true;
        //    this._getElemById('oraCancellazione').disabled = true;
        //    this._checkWaitListAvailable.disabled = true;
        //}
    };


    this._manageSelectStatus = function (elem, enabled) {
        //Dato che non ho trovato un modo funzionante per disabilitare una Select, la distruggo e la ricreo ogni volta
        let instance = M.FormSelect.getInstance(elem);
        //let wrapper = instance.wrapper;
        instance.destroy();
        console.log("Select instance destroyed");
        if (!enabled) {
            elem.setAttribute("disabled", true);
            elem.value = [];
            console.log("Addedd disabled attribute to element");
        } else {
            elem.removeAttribute("disabled");
            console.log("Removed disabled attribute to element");
        }
        M.FormSelect.init(elem);
    };

    this._handleTipoFineRicorrenzaChange = function (event, elemId) {
        if (this._getElemById(elemId).checked) {
            if (elemId === 'rbFineRicorrenzaNum') {
                document.getElementById('numRipetizioni').enabled = true;
                document.getElementById('dataFineRecurrency').enabled = false;
            } else {
                document.getElementById('numRipetizioni').enabled = false;
                document.getElementById('dataFineRecurrency').enabled = true;
            }
        }
    };
}


export const r2dEditEvento = new _r2dEditEvento();