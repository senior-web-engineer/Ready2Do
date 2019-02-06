var EntryPoint =
/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./wwwroot-src/app.js");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./wwwroot-src/app.js":
/*!****************************!*\
  !*** ./wwwroot-src/app.js ***!
  \****************************/
/*! exports provided: r2dEditEvento */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var _materialize_src_sass_materialize_scss__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./materialize-src/sass/materialize.scss */ \"./wwwroot-src/materialize-src/sass/materialize.scss\");\n/* harmony import */ var _materialize_src_sass_materialize_scss__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(_materialize_src_sass_materialize_scss__WEBPACK_IMPORTED_MODULE_0__);\n/* harmony import */ var _css_global_format_scss__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./css/global-format.scss */ \"./wwwroot-src/css/global-format.scss\");\n/* harmony import */ var _css_global_format_scss__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_css_global_format_scss__WEBPACK_IMPORTED_MODULE_1__);\n/* harmony import */ var _css_clienti_scss__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./css/clienti.scss */ \"./wwwroot-src/css/clienti.scss\");\n/* harmony import */ var _css_clienti_scss__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_css_clienti_scss__WEBPACK_IMPORTED_MODULE_2__);\n/* harmony import */ var _js_r2do_EditEvento__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./js/r2do.EditEvento */ \"./wwwroot-src/js/r2do.EditEvento.js\");\n/* harmony reexport (safe) */ __webpack_require__.d(__webpack_exports__, \"r2dEditEvento\", function() { return _js_r2do_EditEvento__WEBPACK_IMPORTED_MODULE_3__[\"r2dEditEvento\"]; });\n\n/* harmony import */ var _js_r2do_RegistrazioneCliente__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./js/r2do.RegistrazioneCliente */ \"./wwwroot-src/js/r2do.RegistrazioneCliente.js\");\n\n\n\n\n\n\n__webpack_require__(/*! ./js/ready2do */ \"./wwwroot-src/js/ready2do.js\");\n\n\n\n//# sourceURL=webpack://EntryPoint/./wwwroot-src/app.js?");

/***/ }),

/***/ "./wwwroot-src/css/clienti.scss":
/*!**************************************!*\
  !*** ./wwwroot-src/css/clienti.scss ***!
  \**************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

eval("// extracted by mini-css-extract-plugin\n\n//# sourceURL=webpack://EntryPoint/./wwwroot-src/css/clienti.scss?");

/***/ }),

/***/ "./wwwroot-src/css/global-format.scss":
/*!********************************************!*\
  !*** ./wwwroot-src/css/global-format.scss ***!
  \********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

eval("// extracted by mini-css-extract-plugin\n\n//# sourceURL=webpack://EntryPoint/./wwwroot-src/css/global-format.scss?");

/***/ }),

/***/ "./wwwroot-src/js/r2do.EditEvento.js":
/*!*******************************************!*\
  !*** ./wwwroot-src/js/r2do.EditEvento.js ***!
  \*******************************************/
/*! exports provided: r2dEditEvento */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"r2dEditEvento\", function() { return r2dEditEvento; });\n/* harmony import */ var _r2do_i18n__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./r2do.i18n */ \"./wwwroot-src/js/r2do.i18n.js\");\n\n\nfunction _r2dEditEvento() {\n  this._selectInstances = [];\n  this._datePickDataEvento = null;\n  this._timePickOraEvento = null;\n  this._datePickCancellabileFinoAl = null;\n  this._timePickCancellabileFinoAl = null;\n  this._datePickAperturaIscriz = null;\n  this._timePickAperturaIscriz = null;\n  this._datePickChiusuraIscriz = null;\n  this._timePickChiusuraIscriz = null;\n  this._datePickInizioVisibIscriz = null;\n  this._timePickInizioVisibIscriz = null;\n  this._datePickFineVisibIscriz = null;\n  this._timePickFineVisibIscriz = null;\n  this._checkCancellazionConsentita = null;\n  this._checkWaitListAvailable = null;\n  this._dataEvento = null;\n  this._tipoRicorrenza = null;\n  /* OPZIONI */\n\n  this._buildOpzioniDatePicker = function (minDate, defaultDate) {\n    var result = {\n      format: \"dd/mm/yyyy\",\n      i18n: _r2do_i18n__WEBPACK_IMPORTED_MODULE_0__[\"default\"].getDateI18N()\n    };\n\n    if (defaultDate) {\n      result.defaultDate = dataEvento;\n      result.setDefaultDate = true;\n    }\n\n    if (minDate) {\n      result.minDate = minDate;\n    }\n\n    return result;\n  };\n\n  this._builOpzioniTimePicker = function () {\n    var result = {\n      twelveHour: false,\n      i18n: _r2do_i18n__WEBPACK_IMPORTED_MODULE_0__[\"default\"].getTimeI18N()\n    };\n    return result;\n  };\n\n  this._getElemById = function (elemId) {\n    var elem = document.getElementById(elemId);\n    return elem;\n  };\n\n  this.init = function (dataEvento, oraEvento) {\n    console.log('EditEvento Init in progress...');\n    var object = this;\n    this._dataEvento = dataEvento; //Iniziliziamo tutte le select\n\n    var selectElems = document.querySelectorAll('select');\n    this._selectInstances = M.FormSelect.init(selectElems); //Iniziliziamo gli Expandables\n\n    var accordionElems = document.querySelectorAll('.collapsible.expandable');\n    M.Collapsible.init(accordionElems, {\n      accordion: false\n    }); //Iniziliaziamo tutti i DatePicker e TimePicker\n\n    this._datePickDataEvento = M.Datepicker.init(this._getElemById('dataEvento'), this._buildOpzioniDatePicker(new Date(), dataEvento));\n    this._datePickCancellabileFinoAl = M.Datepicker.init(this._getElemById('dataCancellazione'), this._buildOpzioniDatePicker(new Date()));\n    this._datePickAperturaIscriz = M.Datepicker.init(this._getElemById('dataAperturaIscrizioni'), this._buildOpzioniDatePicker(new Date()));\n    this._datePickChiusuraIscriz = M.Datepicker.init(this._getElemById('dataChiusuraIscrizioni'), this._buildOpzioniDatePicker(new Date()));\n    this._datePickInizioVisibIscriz = M.Datepicker.init(this._getElemById('visibileDalDate'), this._buildOpzioniDatePicker(new Date()));\n    this._datePickFineVisibIscriz = M.Datepicker.init(this._getElemById('visibileFinoAlDate'), this._buildOpzioniDatePicker(new Date()));\n    this._timePickOraEvento = M.Timepicker.init(this._getElemById('oraEvento'), this._builOpzioniTimePicker());\n    this._timePickCancellabileFinoAl = M.Timepicker.init(this._getElemById('oraCancellazione'), this._builOpzioniTimePicker());\n    this._timePickAperturaIscriz = M.Timepicker.init(this._getElemById('oraAperturaIscrizioni'), this._builOpzioniTimePicker());\n    this._timePickChiusuraIscriz = M.Timepicker.init(this._getElemById('oraChiusuraIscrizioni'), this._builOpzioniTimePicker());\n    this._timePickInizioVisibIscriz = M.Timepicker.init(this._getElemById('visibileDalTime'), this._builOpzioniTimePicker());\n    this._timePickFineVisibIscriz = M.Timepicker.init(this._getElemById('VisibileFinoAlTime'), this._builOpzioniTimePicker()); //CheckBox\n\n    this._checkWaitListAvailable = this._getElemById('waitListDisponibile');\n    this._checkCancellazionConsentita = this._getElemById('cancellazioneConsentita');\n\n    this._checkCancellazionConsentita.addEventListener('change', function (e) {\n      object._handleCancellazioneChange(e);\n    });\n\n    this._handleCancellazioneChange(); //Lo chiamiamo esplicitamente per gestire lo stato iniziale\n    //Ricorrenza\n\n\n    this._tipoRicorrenza = this._getElemById('tipoRicorrenza');\n\n    this._tipoRicorrenza.addEventListener('change', function (e) {\n      object._handleTipoRicorrenzaChange(e);\n    }); //RadiButton Tipologia Fine Ricorrenza\n\n\n    this._getElemById('rbFineRicorrenzaNum').addEventListener('change', function (e) {\n      object._handleTipoFineRicorrenzaChange(e, 'rbFineRicorrenzaNum');\n    });\n\n    this._getElemById('rbFineRicorrenzaDate').addEventListener('change', function (e) {\n      object._handleTipoFineRicorrenzaChange(e, 'rbFineRicorrenzaDate');\n    });\n\n    return 0;\n  };\n\n  this._handleCancellazioneChange = function (event) {\n    if (this._checkCancellazionConsentita.checked) {\n      this._getElemById('dataCancellazione').disabled = false;\n      this._getElemById('oraCancellazione').disabled = false;\n      this._checkWaitListAvailable.disabled = false;\n    } else {\n      this._getElemById('dataCancellazione').disabled = true;\n      this._getElemById('oraCancellazione').disabled = true;\n      this._checkWaitListAvailable.disabled = true;\n    }\n  };\n\n  this._handleTipoRicorrenzaChange = function (event) {\n    var value = this._tipoRicorrenza.value;\n    console.log(\"Inside _handleTipoRicorrenzaChange. Value = \" + value);\n\n    switch (value) {\n      case \"none\":\n        console.log(\"handling value: \" + value);\n\n        this._manageSelectStatus(this._getElemById('weekDaysRecurrency'), false);\n\n        this._getElemById('dataFineRecurrency').enabled = false;\n        this._getElemById('numRipetizioni').enabled = false;\n        this._getElemById('rbFineRicorrenzaNum').enabled = false;\n        this._getElemById('rbFineRicorrenzaDate').enabled = false;\n        break;\n\n      case \"daily\":\n        console.log(\"handling value: \" + value);\n\n        this._manageSelectStatus(this._getElemById('weekDaysRecurrency'), false);\n\n        this._getElemById('dataFineRecurrency').enabled = true;\n        this._getElemById('numRipetizioni').enabled = true;\n        this._getElemById('rbFineRicorrenzaNum').enabled = true;\n        this._getElemById('rbFineRicorrenzaDate').enabled = true;\n        break;\n\n      case \"weekly\":\n        console.log(\"handling value: \" + value);\n\n        this._manageSelectStatus(this._getElemById('weekDaysRecurrency'), true);\n\n        this._getElemById('dataFineRecurrency').enabled = true;\n        this._getElemById('numRipetizioni').enabled = true;\n        this._getElemById('rbFineRicorrenzaNum').enabled = true;\n        this._getElemById('rbFineRicorrenzaDate').enabled = true;\n        break;\n\n      case \"montly\":\n        console.log(\"handling value: \" + value);\n\n        this._manageSelectStatus(this._getElemById('weekDaysRecurrency'), true);\n\n        this._getElemById('dataFineRecurrency').enabled = true;\n        this._getElemById('numRipetizioni').enabled = true;\n        this._getElemById('rbFineRicorrenzaNum').enabled = true;\n        this._getElemById('rbFineRicorrenzaDate').enabled = true;\n        break;\n\n      default:\n        console.log(\"handling ELSE value: \" + value);\n        break;\n    } //if (this._checkCancellazionConsentita.checked) {\n    //    this._getElemById('dataCancellazione').disabled = false;\n    //    this._getElemById('oraCancellazione').disabled = false;\n    //    this._checkWaitListAvailable.disabled = false;\n    //} else {\n    //    this._getElemById('dataCancellazione').disabled = true;\n    //    this._getElemById('oraCancellazione').disabled = true;\n    //    this._checkWaitListAvailable.disabled = true;\n    //}\n\n  };\n\n  this._manageSelectStatus = function (elem, enabled) {\n    //Dato che non ho trovato un modo funzionante per disabilitare una Select, la distruggo e la ricreo ogni volta\n    var instance = M.FormSelect.getInstance(elem); //let wrapper = instance.wrapper;\n\n    instance.destroy();\n    console.log(\"Select instance destroyed\");\n\n    if (!enabled) {\n      elem.setAttribute(\"disabled\", true);\n      elem.value = [];\n      console.log(\"Addedd disabled attribute to element\");\n    } else {\n      elem.removeAttribute(\"disabled\");\n      console.log(\"Removed disabled attribute to element\");\n    }\n\n    M.FormSelect.init(elem);\n  };\n\n  this._handleTipoFineRicorrenzaChange = function (event, elemId) {\n    if (this._getElemById(elemId).checked) {\n      if (elemId === 'rbFineRicorrenzaNum') {\n        document.getElementById('numRipetizioni').enabled = true;\n        document.getElementById('dataFineRecurrency').enabled = false;\n      } else {\n        document.getElementById('numRipetizioni').enabled = false;\n        document.getElementById('dataFineRecurrency').enabled = true;\n      }\n    }\n  };\n}\n\nvar r2dEditEvento = new _r2dEditEvento();\n\n//# sourceURL=webpack://EntryPoint/./wwwroot-src/js/r2do.EditEvento.js?");

/***/ }),

/***/ "./wwwroot-src/js/r2do.RegistrazioneCliente.js":
/*!*****************************************************!*\
  !*** ./wwwroot-src/js/r2do.RegistrazioneCliente.js ***!
  \*****************************************************/
/*! exports provided: r2dRegistrazioneCliente */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"r2dRegistrazioneCliente\", function() { return r2dRegistrazioneCliente; });\nfunction _r2dRegistrazioneCliente() {\n  this._lookupDone = 0;\n\n  this.Init = function (addressElemId) {\n    var input = document.getElementById(addressElemId);\n    input.addEventListener('focus', function () {\n      this._lookupDone = 0;\n    });\n    var options = {\n      type: ['(address)'],\n      componentRestrictions: {\n        country: 'it'\n      }\n    };\n    autocomplete = new google.maps.places.Autocomplete(input, options);\n    autocomplete.addListener('place_changed', function () {\n      var place = autocomplete.getPlace(); //NOTA: se non è stata selezionato uno degli indirizzi proposti da google, sia perché l'utente ha scritto l'inidirizzo e poi ha spostato il focus oppure\n      //      perché c'è stato un problema nel lookup, l'oggetto place ha la sola proprietà Name valorizzta con l'input inserito dall'utente\n      //var log = document.getElementById('log');\n      //log.innerText = log.innerText + '<br>' + 'place_changed EVENT'\n\n      if (place.geometry) {\n        //input.className = input.className.replace(/\\binvalid\\b/g, \"\");\n        ////Applichiamo la classe VALID\n        //arr = input.className.split(\" \");\n        //if (arr.indexOf(\"valid\") == -1) {\n        //    input.className += \" valid\";\n        //}\n        ////input.setCustomValidity(\"\");\n        document.getElementById('esitoLookup').value = 1;\n        document.getElementById('inputLatitudine').value = place.geometry.location.lat();\n        document.getElementById('inputLongitudine').value = place.geometry.location.lng();\n        var tmp = place.address_components.find(function (c) {\n          return c.types[0] === \"postal_code\";\n        });\n\n        if (tmp) {\n          document.getElementById('inputCAP').value = tmp.long_name;\n        }\n\n        tmp = place.address_components.find(function (c) {\n          return c.types[0] === \"locality\";\n        });\n\n        if (tmp) {\n          document.getElementById('inputCountry').value = tmp.long_name;\n        }\n\n        tmp = place.address_components.find(function (c) {\n          return c.types[0] === \"administrative_area_level_1\";\n        });\n\n        if (tmp) {\n          document.getElementById('inputCitta').value = tmp.long_name;\n        } //Dobbiamo rieseguire la validazione perché quando arriva qui è già stata eseguita con i valori \"errati\"\n\n\n        $(\"#formRegistration\").validate().element(\"#addressAutocomplete\");\n      } else {\n        document.getElementById('esitoLookup').value = 0;\n      }\n    });\n  };\n}\n\nvar r2dRegistrazioneCliente = new _r2dRegistrazioneCliente();\n\n//# sourceURL=webpack://EntryPoint/./wwwroot-src/js/r2do.RegistrazioneCliente.js?");

/***/ }),

/***/ "./wwwroot-src/js/r2do.i18n.js":
/*!*************************************!*\
  !*** ./wwwroot-src/js/r2do.i18n.js ***!
  \*************************************/
/*! exports provided: default */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* Internazionalizzazione */\nfunction MaterilizeI18N() {\n  var datePickerI18n = {\n    \"it\": {\n      months: ['Gennaio', 'Febbraio', 'Marzo', 'Aprile', 'Maggio', 'Giugno', 'Luglio', 'Agosto', 'Settembre', 'Ottobre', 'Novembre', 'Dicembre'],\n      monthsShort: ['Gen', 'Feb', 'Mar', 'Apr', 'Mag', 'Giu', 'Lug', 'Ago', 'Set', 'Ott', 'Nov', 'Dic'],\n      weekdays: ['Domenica', 'Lunedì', 'Martedì', 'Mercoledì', 'Giovedì', 'Venerdì', 'Sabato'],\n      weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mer', 'Gio', 'Ven', 'Sab', 'Dom'],\n      weekdaysAbbrev: ['D', 'L', 'M', 'M', 'G', 'V', 'S'],\n      clear: 'Cancella',\n      cancel: 'Annulla',\n      today: 'Oggi'\n    }\n  };\n  var timePickeri18n = {\n    \"it\": {\n      cancel: 'Annulla',\n      clear: 'Cancella',\n      done: 'Ok'\n    }\n  };\n\n  this.getDateI18N = function (local) {\n    //Per ora gestiamo solo l'italiano\n    return datePickerI18n[\"it\"];\n  };\n\n  this.getTimeI18N = function (local) {\n    //Per ora gestiamo solo l'italiano\n    return timePickeri18n[\"it\"];\n  };\n}\n\n;\n/* harmony default export */ __webpack_exports__[\"default\"] = (new MaterilizeI18N()); //const r2sMatI18N =\n\n//# sourceURL=webpack://EntryPoint/./wwwroot-src/js/r2do.i18n.js?");

/***/ }),

/***/ "./wwwroot-src/js/ready2do.js":
/*!************************************!*\
  !*** ./wwwroot-src/js/ready2do.js ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports) {

eval("//Modulo parent (empty)\nvar ready2do = function () {\n  return {};\n}();\n\n//# sourceURL=webpack://EntryPoint/./wwwroot-src/js/ready2do.js?");

/***/ }),

/***/ "./wwwroot-src/materialize-src/sass/materialize.scss":
/*!***********************************************************!*\
  !*** ./wwwroot-src/materialize-src/sass/materialize.scss ***!
  \***********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

eval("// extracted by mini-css-extract-plugin\n\n//# sourceURL=webpack://EntryPoint/./wwwroot-src/materialize-src/sass/materialize.scss?");

/***/ })

/******/ });