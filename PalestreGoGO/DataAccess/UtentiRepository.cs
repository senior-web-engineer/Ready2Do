using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess.Interfaces;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dapper;


namespace PalestreGoGo.DataAccess
{
    public class UtentiRepository: IUtentiRepository
    {
       
        private readonly PalestreGoGoDbContext _context;
        private readonly ILogger<UtentiRepository> _logger;

        public UtentiRepository(PalestreGoGoDbContext context, ILogger<UtentiRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ClienteFollowed>> GetGlientiFollowedAsync(Guid userId)
        {
            IEnumerable<ClienteFollowed> result = null;
            using (var cn = _context.Database.GetDbConnection())
            {
                result = await cn.QueryAsync<ClienteFollowed>(StoredProcedure.SP_USER_CLIENTI_FOLLOWED, 
                                                            new { pUserId = userId}, 
                                                            commandType: System.Data.CommandType.StoredProcedure);
            }
            return result?.AsList();
        }
    }
}
