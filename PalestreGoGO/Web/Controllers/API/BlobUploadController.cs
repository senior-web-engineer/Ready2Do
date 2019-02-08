using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models;
using Web.Proxies;
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
        private readonly ClienteProxy _apiClient;

        public BlobUploadController(ILogger<BlobUploadController> logger,
                                    IOptions<AppConfig> apiOptions,
                                    ClienteProxy apiClient)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
        }


        [HttpGet]
        [Produces("text/plain")] //Necessario altrimenti ritorna JSON e FineUploader costruisce male l'URL per azure
        //Accesso anonimo ma verifichiamo l'header custom
        //[AllowAnonymous]
        public async Task<IActionResult> GetBlobSAS([FromQuery]string _method, [FromQuery]string bloburi, [FromQuery] string qqtimestamp)
        {
            if (!Uri.IsWellFormedUriString(bloburi, UriKind.Absolute)) { return BadRequest(); }
            if (!HttpContext.Request.Headers.ContainsKey(Constants.CUSTOM_HEADER_TOKEN_AUTH)) { return BadRequest(); }
            var headers = HttpContext.Request.Headers[Constants.CUSTOM_HEADER_TOKEN_AUTH];
            if (!TryParseToken(headers.FirstOrDefault(), out var token)) { return Forbid(); }
            //Verificare che l'utente specificato (tramite il securityToken sia l'owner del container)
            var cliente = await _apiClient.GetClienteAsync(token.IdCliente);
            if ((cliente == null) || (!token.ContainerName.Equals(cliente.StorageContainer)))
            {
                _logger.LogCritical("Token COMPROMESSO. Il container name non coincide con quello dell'utente");
                return Forbid();
            }
            //Verifichiamo che l'URI del blob utilizzi il container di proprietà dell'utente identificato dal token
            CloudBlob azureBlob = new CloudBlob(new Uri(bloburi));
            if (!azureBlob.Container.Name.Equals(token.ContainerName)) { return Forbid(); }
            await AzureStorageUtils.EnsureContainerExists(_appConfig.Azure, token.ContainerName);
            var sas = AzureStorageUtils.GetSasForBlob(_appConfig.Azure, bloburi, _method);
            return Ok(sas);
        }

        [HttpPost]
        public async Task<IActionResult> FileUploaded(string blob, string uuid, string name, string container, int fileOrder, int imageId)
        {
            if (!Uri.IsWellFormedUriString(container, UriKind.Absolute)) { return BadRequest(); }
            if (!HttpContext.Request.Headers.ContainsKey(Constants.CUSTOM_HEADER_TOKEN_AUTH)) { return BadRequest(); }
            var headers = HttpContext.Request.Headers[Constants.CUSTOM_HEADER_TOKEN_AUTH];
            if (!TryParseToken(headers.FirstOrDefault(), out var token)) { return Forbid(); }
            ////Verificare che l'utente specificato (tramite il securityToken sia l'owner del container)
            var cliente = await _apiClient.GetClienteAsync(token.IdCliente);
            if ((cliente == null) || (!token.ContainerName.Equals(cliente.StorageContainer)))
            {
                _logger.LogCritical("Token COMPROMESSO. Il container name non coincide con quello dell'utente");
                return Forbid();
            }
            //Verifichiamo che l'URI del blob utilizzi il container di proprietà dell'utente identificato dal token
            string url = $"{container}/{blob}";
            CloudBlob azureBlob = new CloudBlob(new Uri(url));
            if (!azureBlob.Container.Name.Equals(token.ContainerName)) { return Unauthorized(); }
            //TODO: Verificare che l'id immagine sia valorizzata correttamente 
            //Salviamo l'informazione sull'immagine caricata nel DB
            ImmagineClienteDM immagine = new ImmagineClienteDM()
            {
                Id = imageId >= 0 ? imageId : (int?)null,
                Nome = name,
                Ordinamento = fileOrder,
                Url = url
            };
            await _apiClient.GallerySalvaImmagine(cliente.Id.Value, immagine);
            return Ok(url);
        }


        private bool TryParseToken(string token, out SASTokenModel sasToken)
        {
            sasToken = null;
            if (string.IsNullOrWhiteSpace(token)) { return false; }
            try
            {
                string json = SecurityUtils.DecryptStringFromBytes_Aes(Convert.FromBase64String(token), Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
                sasToken = JsonConvert.DeserializeObject<SASTokenModel>(json);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Errore durante decodifica del token SAS.");
                return false;
            }
            if (DateTime.Now.Subtract(sasToken.CreationTime).TotalMinutes > _appConfig.AuthTokenDuration)
            {
                _logger.LogWarning("SASToken scaduto. {0}", token);
                return false;
            }
            return true;
        }

        [HttpDelete]
        public IActionResult DeleteFile()
        {
            throw new NotImplementedException();
            //return Ok();
        }
    }
}