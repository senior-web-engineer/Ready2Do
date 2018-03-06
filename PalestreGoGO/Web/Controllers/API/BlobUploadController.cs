using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Web.Configuration;
using Web.Models;
using Web.Utils;

namespace Web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/FileUpload")]
    [Authorize]
    public class BlobUploadController : ControllerBase
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger<BlobUploadController> _logger;

        public BlobUploadController(ILogger<BlobUploadController> logger,
                                    IOptions<AppConfig> apiOptions)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
        }


        [HttpGet]
        [Produces("text/plain")] //Necessario altrimenti ritorna JSON e FineUploader costruisce male l'URL per azure
        //Accesso anonimo ma verifichiamo l'header custom
        [AllowAnonymous]
        public async Task<IActionResult> GetBlobSAS([FromQuery]string _method, [FromQuery]string bloburi, [FromQuery] string qqtimestamp)
        {
            SASTokenModel token;

            if (!Uri.IsWellFormedUriString(bloburi, UriKind.Absolute))
            {
                return BadRequest();
            }
            if (!HttpContext.Request.Headers.ContainsKey(Constants.CUSTOM_HEADER_TOKEN_AUTH))
            {
                return BadRequest();
            }
            var header = HttpContext.Request.Headers[Constants.CUSTOM_HEADER_TOKEN_AUTH];
            var headerValue = header.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return Unauthorized();
            }
            try {
                string json = SecurityUtils.DecryptStringFromBytes_Aes(Convert.FromBase64String(headerValue), Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
                token = JsonConvert.DeserializeObject<SASTokenModel>(json);
            }catch(Exception exc)
            {
                _logger.LogError(exc, "Errore durante decodifica del token SAS.");
                return Unauthorized();
            }
            
            //TODO: Implementare la verifica dei dati contenuti nel token
            if(DateTime.Now.Subtract(token.CreationTime).TotalMinutes > _appConfig.AuthTokenDuration)
            {
                _logger.LogWarning("SASToken scaduto. {0}", token);
                return Unauthorized();
            }

            //Verificare che l'utente specificato (tramite il securityToken sia l'owner del container)
            var cliente = await WebAPIClient.GetClienteFromTokenAsync(token.SecurityToken, _appConfig.WebAPI.BaseAddress);
            if((cliente == null) || (!token.ContainerName.Equals(cliente.StorageContainer)))
            {
                _logger.LogCritical("Token COMPROMESSO. Il container name non coincide con quello dell'utente");
                return Unauthorized();
            }

            //Verifichiamo che l'URI del blob utilizzi il container di proprietà dell'utente identificato dal token
            CloudBlob azureBlob = new CloudBlob(new Uri(bloburi));
            if (!azureBlob.Container.Name.Equals(token.ContainerName))
            {
                return Unauthorized();
            }
            var sas = AzureStorageUtils.GetSasForBlob(_appConfig.Azure, bloburi, _method);
            //TODO: Costruire il token SAS, vedere la documentazione del componente di UPLOAD
            return Ok(sas);
        }

        [HttpPost]
        public async Task<IActionResult> FileUploaded(string blob, string uuid, string name, string container, int fileOrder)
        {
            string reqBody;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                reqBody = await reader.ReadToEndAsync();
            }

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFile()
        {
            return Ok();
        }
    }
}