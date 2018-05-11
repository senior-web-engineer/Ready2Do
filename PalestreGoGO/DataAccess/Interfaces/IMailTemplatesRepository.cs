using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess.Interfaces
{
    public interface IMailTemplatesRepository
    {
        Task<MailTemplate> GetTemplateAsync(MailType tipoMail);
    }
}
