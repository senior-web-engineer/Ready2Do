using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PalestreGoGo.DataModel;

namespace PalestreGoGo.DataAccess
{
    public interface IUtentiRepository
    {
        Task<List<ClienteFollowed>> GetGlientiFollowedAsync(Guid userId);
    }
}
