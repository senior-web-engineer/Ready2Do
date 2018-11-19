using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Services;
using Web.Utils;

namespace Web.ViewComponents
{
    public class NotificheViewComponent : ViewComponent
    {
        //private readonly WebAPIClient _apiClient;
        //private readonly ILogger<NotificheViewComponent> _logger;
        //private readonly ClienteResolverServices _clientiResolver;

        //public NotificheViewComponent( WebAPIClient apiClient, 
        //                               ILogger<NotificheViewComponent> logger,
        //                               ClienteResolverServices clientiResolver)
        //{
        //    _apiClient = apiClient;
        //    _logger = logger;
        //    _clientiResolver = clientiResolver;
        //}

        public IViewComponentResult Invoke(int? idCliente, List<NotificaViewModel> notifiche)
        {
            //var accessToken = await HttpContext.GetTokenAsync("access_token");
            //var vm = await _apiClient.GetNotificheForUserAsync(accessToken);
            return View(notifiche);
        }

    }
}
