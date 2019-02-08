using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Proxies
{
    public class TipologicheProxy: BaseAPIProxy
    {
        #region TIPOLOGIE CLIENTE

        public async Task<IEnumerable<TipologiaClienteDM>> GetTipologieClientiAsync()
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/tipologie");
            return await GetRequestAsync<IEnumerable<TipologiaClienteDM>>(uri, false);
        }

        #endregion

        #region LOCATIONS

        public async Task<IEnumerable<LocationDM>> GetLocationsAsync(int idCliente)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations");
            return await GetRequestAsync<IEnumerable<LocationDM>>(uri, false);
        }

        public async Task SaveLocationAsync(int idCliente, LocationInputDM location)
        {
            if (location.Id.HasValue && location.Id.Value > 0)
            {
                await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{location.Id}", location);
            }
            else
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations", location);
            }
        }

        public async Task<LocationInputDM> GetOneLocationAsync(int idCliente, int idLocation)
        {
            return await GetRequestAsync<LocationInputDM>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{idLocation}"));
        }

        public async Task DeleteOneLocationAsync(int idCliente, int idLocation)
        {
            await DeleteRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{idLocation}");
        }
        #endregion

        #region TIPOLOGIE LEZIONI
        public async Task SaveTipologiaLezioneAsync(int idCliente, TipologiaLezioneDM tipoLezione)
        {
            if (tipoLezione.Id.HasValue && tipoLezione.Id.Value > 0)
            {
                await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni/{tipoLezione.Id}", tipoLezione);
            }
            else
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni", tipoLezione);
            }
        }

        public async Task<IEnumerable<TipologiaLezioneDM>> GetTipologieLezioniClienteAsync(int idCliente)
        {
            return await GetRequestAsync<IEnumerable<TipologiaLezioneDM>>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni"), false);
        }

        public async Task<TipologiaLezioneDM> GetOneTipologiaLezione(int idCliente, int idTipologia)
        {
            return await GetRequestAsync<TipologiaLezioneDM>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni/{idTipologia}"));
        }

        public async Task<bool> CheckNameTipologiaLezioneAsync(int idCliente, string nome, int? id)
        {
            string queryString = id.HasValue ? $"?id={id}" : "";
            return await GetRequestAsync<bool>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni/checkname/{nome}{queryString}"));
        }

        public async Task DeleteOneTipologiaLezioneAsync(int idCliente, int idLocation)
        {
            await DeleteRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni/{idLocation}");
        }

        #endregion

        #region TIPOLOGIE ABBONAMENTI
        public async Task<IEnumerable<TipologiaAbbonamentoDM>> GetTipologieAbbonamentiClienteAsync(int idCliente)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti");
            return await GetRequestAsync<IEnumerable<TipologiaAbbonamentoDM>>(uri);
        }

        public async Task<TipologiaAbbonamentoDM> GetOneTipologiaAbbonamentoAsync(int idCliente, int idTipoAbbonamento)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti/{idTipoAbbonamento}");
            return await GetRequestAsync<TipologiaAbbonamentoDM>(uri);
        }

        public async Task SaveTipologiaAbbonamentoAsync(int idCliente, TipologiaAbbonamentoInputDM tipoAbbonamento)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti");
            if (tipoAbbonamento.Id.HasValue && tipoAbbonamento.Id.Value > 0)
            {
                await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti/{tipoAbbonamento.Id}", tipoAbbonamento);
            }
            else
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti", tipoAbbonamento);
            }
        }

        public async Task DeleteOneTipologiaAbbonamentoAsync(int idCliente, int idTipoAbbonamento)
        {
            await DeleteRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti/{idTipoAbbonamento}");
        }

        public async Task<bool> CheckNomeTipologiaAbbonamentoAsync(int idCliente, string nome, int? id)
        {
            string qsOpz = id.HasValue ? $"&id={id}" : "";
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti/checknome?nome={nome}{qsOpz}");
            return await GetRequestAsync<bool>(uri);
        }

        #endregion

    }
}
