using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Snai.CMS.Api_Core.Business;
using Snai.CMS.Api_Core.Controllers;
using Snai.CMS.Api_Core.DataAccess;
using Snai.CMS.Api_Core.Entities.Settings;
using System.Security.Claims;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Auth
{
    public class PermissionHandler: AuthorizationHandler<PermissionRequirement>
    {
        CMSBO _cmsBO;

        public PermissionHandler(CMSBO cmsBO)
        {
            _cmsBO = cmsBO;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // 取token,验证是否退出
            var request = context.Resource as HttpRequest;
            if (!request.Headers.TryGetValue("Authorization", out var tokenBearer))
            {
                context.Fail();
                return; 
            }

            var tokenBearerStr = tokenBearer.ToString();
            if (!tokenBearerStr.StartsWith("Bearer"))
            {
                context.Fail();
                return;
            }

            var tokenArrays = tokenBearerStr.Split(" ");
            if (tokenArrays == null || tokenArrays.Length != 2)
            {
                context.Fail();
                return;
            }

            var tokenStr = tokenArrays[1];
            var token = _cmsBO.GetToken(tokenStr);
            if (token == null || token.State == 2) 
            {
                context.Fail();
                return;
            }

            // 验证权限
            var resource = ((Microsoft.AspNetCore.Routing.RouteEndpoint)context.Resource).RoutePattern;

            foreach (var tc in context.User.Identities)
            {
                foreach(var claim in tc.Claims)
                {
                    if (claim.Type == ClaimTypes.Name)
                    { 
                        var userName = claim.Value;
                        var router = (resource.RawText??"").ToLower();

                        var msg = _cmsBO.VerifyUserRole(userName, router);
                        if (msg.Code == (int)Code.Success) 
                        {
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }
            }

            context.Fail();
            return;
        }
    }
}
