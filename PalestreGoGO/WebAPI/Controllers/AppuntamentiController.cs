using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PalestreGoGo.WebAPI.Controllers;

namespace PalestreGoGo.WebAPI.Controllers
{
    /// <summary>
    /// Gestione appuntamenti
    /// </summary>
    /// <seealso cref="https://palestregogo.visualstudio.com/MyFirstProject/_wiki?pagePath=%2FAppuntamenti"/>
    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/appuntamenti")]
    public class AppuntamentiController : PalestreControllerBase
    {
        
        /* 
         1. Get -> Ritorna i dettagli di un appuntamento 
                INOCABILE DA:
                    - Gestore Palestra
                    - Gestore dello Schedule (per ora non lo gestiamo)
                    - Utente owner dell'appuntamento:
         2. Post -> Crea un nuovo appuntamento
                INVOCABILE DA:
                    - utente registrato 
         */
    }
}