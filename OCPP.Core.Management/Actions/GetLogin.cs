
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Unicode;

namespace OCPP.Core.Management.Actions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class GetLogin : Attribute, IAuthorizationFilter
    {
        private readonly IConfiguration configuration;

        public GetLogin(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var auth = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(auth))
            {
                context.Result = new UnauthorizedObjectResult(string.Empty);
            }

            auth = auth.Split(' ')[1];
            auth = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
            var username = auth.Split(":")[0];
            var password = auth.Split(":")[1];

            if (!(username == configuration.GetValue<string>("ApiUserName") && password == configuration.GetValue<string>("ApiPassword")))
            {
                context.Result = new UnauthorizedObjectResult(string.Empty);
            }
        }

    }
}
