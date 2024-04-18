using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Snai.CMS.Api_Core.Business;
using Snai.CMS.Api_Core.Common.Infrastructure.Extension;
using Snai.CMS.Api_Core.Controllers;
using Snai.CMS.Api_Core.DataAccess;
using Snai.CMS.Api_Core.Entities.Settings;
using System.Security.Claims;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Auth
{
    public class PermissionHandler: AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger<HomeController> _logger;
        HttpContextExtension _httpContext;
        Consts _consts;
        CMSBO _cmsBO;

        public PermissionHandler(ILogger<HomeController> logger, HttpContextExtension httpContext, Consts consts, CMSBO cmsBO)
        {
            _cmsBO = cmsBO;
            _httpContext = httpContext;
            _consts = consts;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // 取token和router验证权限
            // 是否经过验证
            var isAuthenticated = context.User.Identity.IsAuthenticated;
            if (isAuthenticated)
            {
                var tokenStr = _httpContext.GetToken();
                if (string.IsNullOrEmpty(tokenStr))
                {
                    context.Fail();
                    var msg = new Message() { Code = (int)Code.AuthNotExist, Msg = _consts.GetMsg(Code.AuthNotExist) };
                    _httpContext.OutHttpResult(msg);
                    return;
                }

                var token = _cmsBO.GetToken(tokenStr);
                if (token == null || token.State == 2)
                {
                    context.Fail();
                    var msg = new Message() { Code = (int)Code.AuthCheckFail, Msg = _consts.GetMsg(Code.AuthCheckFail) };
                    _httpContext.OutHttpResult(msg);
                    return;
                }

                // 验证权限
                //var resource = ((Microsoft.AspNetCore.Routing.RouteEndpoint)context.Resource).RoutePattern;

                foreach (var tc in context.User.Identities)
                {
                    foreach (var claim in tc.Claims)
                    {
                        if (claim.Type == ClaimTypes.Name)
                        {
                            var userName = claim.Value;
                            var router = _httpContext.GetRouterPath(); //(resource.RawText??"").ToLower();

                            var msg = _cmsBO.VerifyUserRole(userName, router);
                            if (msg.Code == (int)Code.Success)
                            {
                                context.Succeed(requirement);
                                return;
                            }
                        }
                    }
                }
            }

            context.Fail();
            return;
        }
    }
}
