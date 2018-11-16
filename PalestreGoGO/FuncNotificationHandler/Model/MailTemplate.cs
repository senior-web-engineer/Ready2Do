using System;
using System.Collections.Generic;
using System.Text;

namespace FuncNotificationHandler
{
    public class MailTemplate
    {
        public int Id { get; set; }
        public string TipoMail { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
        public bool OnlyText { get; set; }
        public DateTime? DataCancellazione { get; set; }
    }
}
