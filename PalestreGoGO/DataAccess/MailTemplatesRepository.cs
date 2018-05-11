using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess.Interfaces;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class MailTemplatesRepository : IMailTemplatesRepository
    {
        private readonly PalestreGoGoDbContext _context;
        private readonly ILogger<MailTemplatesRepository> _logger;

        public MailTemplatesRepository(PalestreGoGoDbContext context, ILogger<MailTemplatesRepository> logger)
        {
            this._logger = logger;
            this._context = context;
        }


        public async Task<MailTemplate> GetTemplateAsync(MailType tipoMail)
        {
            return await _context.MailTemplates.SingleOrDefaultAsync(m => m.TipoMail == (byte)tipoMail);
        }
    }
}
