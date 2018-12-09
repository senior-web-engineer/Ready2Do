using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace PalestreGoGo.DataAccess
{
    public class MailTemplatesRepository : BaseRepository, IMailTemplatesRepository
    {
        public MailTemplatesRepository(IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<MailTemplate> GetTemplateAsync(MailType tipoMail)
        {
            using (var cn = GetConnection())
            {
                return await cn.QuerySingleAsync<MailTemplate>("SELECT * FROM MailTemplates WHERE TipoMail = @pTipoMail", new { pTipoMail = (byte)tipoMail });
            }
        }
    }
}
