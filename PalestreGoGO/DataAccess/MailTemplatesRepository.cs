using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace PalestreGoGo.DataAccess
{
    public class MailTemplatesRepository : IMailTemplatesRepository
    {
        //private readonly PalestreGoGoDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailTemplatesRepository> _logger;

        public MailTemplatesRepository(IConfiguration configuration, ILogger<MailTemplatesRepository> logger)
        {
            this._logger = logger;
            //this._context = context;
            this._configuration = configuration;
        }


        public async Task<MailTemplate> GetTemplateAsync(MailType tipoMail)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await cn.QuerySingleAsync<MailTemplate>("SELECT * FROM MailTemplates WHERE TipoMail = @pTipoMail", new { pTipoMail = (byte)tipoMail });
                //return await _context.MailTemplates.SingleOrDefaultAsync(m => m.TipoMail == (byte)tipoMail);
            }
        }
    }
}
