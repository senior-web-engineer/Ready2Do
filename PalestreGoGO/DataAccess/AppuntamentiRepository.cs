using System;
using System.Collections.Generic;
using System.Text;
using PalestreGoGo.DataModel;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel.Exceptions;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;

namespace PalestreGoGo.DataAccess
{
    public class AppuntamentiRepository : IAppuntamentiRepository
    {
        private readonly PalestreGoGoDbContext _context;
        private readonly ILogger<AppuntamentiRepository> _logger;
        private readonly IConfiguration _config;

        public AppuntamentiRepository(ILogger<AppuntamentiRepository> logger, PalestreGoGoDbContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        public async Task<int> AddAppuntamentoAsync(int idCliente, Appuntamenti appuntamento)
        {
            if (appuntamento == null) throw new ArgumentNullException(nameof(appuntamento));
            if (!appuntamento.IdCliente.Equals(idCliente)) throw new ArgumentException("Wrong Tenant");
            using(var cn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var retVal = new SqlParameter();
                retVal.Direction = ParameterDirection.ReturnValue;
                var parId = new SqlParameter("@pId", SqlDbType.Int);
                parId.Direction = ParameterDirection.Output;

                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Add]";
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 50).Value = appuntamento.UserId?.ToString();
                cmd.Parameters.Add("@pScheduleId", SqlDbType.Int).Value = appuntamento.ScheduleId;
                cmd.Parameters.Add("@pIdAbbonamento", SqlDbType.Int).Value = appuntamento.IdAbbonamento;
                cmd.Parameters.Add("@pNote", SqlDbType.NVarChar, 1000).Value = appuntamento.Note;
                cmd.Parameters.Add("@pNominativo", SqlDbType.NVarChar, 200).Value = appuntamento.Nominativo;
                cmd.Parameters.Add(parId);
                cmd.Parameters.Add(retVal);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                appuntamento.Id = (int)parId.Value;
            }

            return appuntamento.Id;
        }

        public async Task CancelAppuntamentoAsync(int idCliente, int idAppuntamento)
        {

            using (var cn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var retVal = new SqlParameter();
                retVal.Direction = ParameterDirection.ReturnValue;
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Delete]";
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdAppuntamento", SqlDbType.Int).Value = idAppuntamento;
                cmd.Parameters.Add(retVal);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                //if((int)retVal.Value != 1)
                //{
                //    throw new InvalidOperationException("Impossibile annullare l'appuntamento.");
                //}
            }
        }



        public IEnumerable<Appuntamenti> GetAppuntamentiForSchedule(int idCliente, int idSchedule)
        {
            var appuntamenti = _context.Appuntamenti.Where(a => a.Id.Equals(idSchedule) && a.IdCliente.Equals(idCliente));
            return appuntamenti;
        }

        public async Task<Appuntamenti> GetAppuntamentoAsync(int idCliente, int idAppuntamento)
        {
            return await _context.Appuntamenti.Where(a => a.IdCliente.Equals(idCliente) && a.Id.Equals(idAppuntamento)).FirstOrDefaultAsync();
        }

        public async Task<Appuntamenti> GetAppuntamentoForScheduleAsync(int idCliente, int idSchedule, string userId)
        {
            return await _context.Appuntamenti.Where(a => a.IdCliente.Equals(idCliente) && a.ScheduleId.Equals(idSchedule) && a.UserId.Equals(userId)).FirstOrDefaultAsync();
        }

        public IEnumerable<Appuntamenti> GetAppuntamentiForUser(int idCliente, string userId, bool includePast=false)
        {
            return _context
                    .Appuntamenti
                    .AsNoTracking()
                    .Include(a => a.Cliente)
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.TipologiaLezione)
                    .Where(a => a.IdCliente.Equals(idCliente) && a.UserId.Equals(userId) );
        }

        public IEnumerable<Appuntamenti> GetAppuntamentiForUser(string userId, bool includePast)
        {
            return _context
                    .Appuntamenti
                    .AsNoTracking()
                    .Include(a => a.Cliente)
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.TipologiaLezione)
                    .Where(a => a.UserId.Equals(userId) && (!includePast || a.Schedule.Data > DateTime.Now));
        }
    }
}
