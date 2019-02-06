function _r2dRegistrazioneCliente() {
    this._lookupDone = 0;

    this.Init = function (addressElemId) {
        var input = document.getElementById(addressElemId);

        input.addEventListener('focus', function () {
            this._lookupDone = 0;
        });

        var options = {
            type: ['(address)'],
            componentRestrictions: { country: 'it' }
        };

        autocomplete = new google.maps.places.Autocomplete(input, options);
        autocomplete.addListener('place_changed', function () {
            var place = autocomplete.getPlace();
            //NOTA: se non è stata selezionato uno degli indirizzi proposti da google, sia perché l'utente ha scritto l'inidirizzo e poi ha spostato il focus oppure
            //      perché c'è stato un problema nel lookup, l'oggetto place ha la sola proprietà Name valorizzta con l'input inserito dall'utente

            //var log = document.getElementById('log');
            //log.innerText = log.innerText + '<br>' + 'place_changed EVENT'
            if (place.geometry) {
                //input.className = input.className.replace(/\binvalid\b/g, "");
                ////Applichiamo la classe VALID
                //arr = input.className.split(" ");
                //if (arr.indexOf("valid") == -1) {
                //    input.className += " valid";
                //}
                ////input.setCustomValidity("");
                document.getElementById('esitoLookup').value = 1;
                document.getElementById('inputLatitudine').value = place.geometry.location.lat();
                document.getElementById('inputLongitudine').value = place.geometry.location.lng();

                var tmp = place.address_components.find(function (c) { return c.types[0] === "postal_code"; });
                if (tmp) {
                    document.getElementById('inputCAP').value = tmp.long_name;
                }
                tmp = place.address_components.find(function (c) { return c.types[0] === "locality"; });
                if (tmp) {
                    document.getElementById('inputCountry').value = tmp.long_name;
                }
                tmp = place.address_components.find(function (c) { return c.types[0] === "administrative_area_level_1"; });
                if (tmp) {
                    document.getElementById('inputCitta').value = tmp.long_name;
                }
                //Dobbiamo rieseguire la validazione perché quando arriva qui è già stata eseguita con i valori "errati"
                $("#formRegistration").validate().element("#addressAutocomplete");
            } else {
                document.getElementById('esitoLookup').value = 0;
            }
        });
    };
}


export const r2dRegistrazioneCliente = new _r2dRegistrazioneCliente();