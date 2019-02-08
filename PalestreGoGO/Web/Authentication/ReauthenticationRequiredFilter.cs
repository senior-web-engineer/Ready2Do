﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Authentication
{
    internal class ReauthenticationRequiredFilter : IExceptionFilter
    {
        private readonly B2CPolicies policies;

        public ReauthenticationRequiredFilter(IOptions<B2CPolicies> policies)
        {
            this.policies = policies.Value;
        }

        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled && IsReauthenticationRequired(context.Exception))
            {
                context.Result = new ChallengeResult(
                        Constants.OpenIdConnectAuthenticationScheme,
                        new AuthenticationProperties(new Dictionary<string, string> { { Constants.B2CPolicy, policies.SignInOrSignUpPolicy } })
                        {
                            RedirectUri = context.HttpContext.Request.Path
                        });

                context.ExceptionHandled = true;
            }
        }

        private static bool IsReauthenticationRequired(Exception exception)
        {
            if (exception is ReauthenticationRequiredException)
            {
                return true;
            }

            if (exception.InnerException != null)
            {
                return IsReauthenticationRequired(exception.InnerException);
            }

            return false;
        }
    }
}
