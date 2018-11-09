using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class NotificaConTipoApiModel : NotificaApiModel
    {
        public TipologiaNotificaApiModel Tipo { get; set; }
    }
}
