using Newtonsoft.Json;
using Snai.CMS.Api_Core.Business;
using Snai.CMS.Api_Core.Common.Infrastructure.Extension;
using System.Security.Claims;
using System.Text;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Middleware
{
    public class AuthorizeStaticFilesMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizeStaticFilesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, CMSBO cmsBO)
        {
            try
            {
                var isAuthenticated = context.User.Identity.IsAuthenticated;
                if (isAuthenticated)
                {
                    if (!context.Request.Headers.TryGetValue("Authorization", out var tokenBearer))
                    {
                        var result = JsonConvert.SerializeObject(new { Code = "401", Message = "您无权查看该文件Authorization" });
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync(result);
                        return;
                    }

                    var tokenBearerStr = tokenBearer.ToString();
                    if (!tokenBearerStr.StartsWith("Bearer"))
                    {
                        var result = JsonConvert.SerializeObject(new { Code = "401", Message = "您无权查看该文件Bearer" });
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync(result);
                        return;
                    }

                    var tokenArrays = tokenBearerStr.Split(" ");
                    if (tokenArrays == null || tokenArrays.Length != 2)
                    {
                        var result = JsonConvert.SerializeObject(new { Code = "401", Message = "您无权查看该文件Arrays2" });
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync(result);
                        return;
                    }

                    var tokenStr = tokenArrays[1];

                    if (string.IsNullOrEmpty(tokenStr))
                    {
                        var result = JsonConvert.SerializeObject(new { Code = "401", Message = "您无权查看该文件Empty" });
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync(result);
                        return;
                    }

                    var token = cmsBO.GetToken(tokenStr);
                    if (token == null || token.State == 2)
                    {
                        var result = JsonConvert.SerializeObject(new { Code = "401", Message = "您无权查看该文件State" });
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync(result);
                        return;
                    }

                    await _next(context);
                }
                else
                {
                    var result = JsonConvert.SerializeObject(new { Code = "401", Message = "您无权查看该文件" });
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(result);
                    return;
                }
            }
            catch (Exception e)
            { }
        }
    }
}
