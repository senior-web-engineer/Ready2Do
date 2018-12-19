﻿using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ITipologieAbbonamentiRepository
    {
        Task<Tuple<IEnumerable<TipologiaAbbonamentoDM>, int>> GetListAsync(int idCliente, int pageSize = 25, int pageNumber = 1, int? id = null,
                                                                         string sortColumn = "DataCreazione", bool sortAscending = false,
                                                                         bool includiCancellati = false, bool includiNonAttivi = false,
                                                                         DateTime? dataValutazione = null);
        Task<TipologiaAbbonamentoDM> GetOneAsync(int idCliente, int id);
        Task<int> AddAsync(int idCliente, TipologiaAbbonamentoDM entity);
        Task UpdateAsync(int idCliente, TipologiaAbbonamentoDM entity);
        Task DeleteAsync(int idCliente, int id);
        Task<bool> CheckNomeAsync(int idCliente, string nome);
    }
}
