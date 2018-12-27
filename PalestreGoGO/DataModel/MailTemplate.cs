using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{

    public enum MailType : byte
    {
        ConfermaCliente = 1,
        ConfermaUtente = 2
    }

    public class MailTemplate
    {
        public short Id { get; set; }
        public byte TipoMail { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
    }
}
