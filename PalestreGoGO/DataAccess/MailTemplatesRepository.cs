using Dapper;
using Microsoft.Extensions.Configuration;
using PalestreGoGo.DataModel;
using System.Threading.Tasks;

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
