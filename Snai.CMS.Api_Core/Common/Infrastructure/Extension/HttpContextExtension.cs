using Microsoft.AspNetCore.Mvc;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Extension
{
    public class HttpContextExtension
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private HttpContext Context => HttpContextAccessor.HttpContext;

        #region 构造函数    
        public HttpContextExtension(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region 常用方法

        // 取token
        public string GetToken()
        {
            if (!Context.Request.Headers.TryGetValue("Authorization", out var tokenBearer))
            {
                return "";
            }

            var tokenBearerStr = tokenBearer.ToString();
            if (!tokenBearerStr.StartsWith("Bearer"))
            {
                return "";
            }

            var tokenArrays = tokenBearerStr.Split(" ");
            if (tokenArrays == null || tokenArrays.Length != 2)
            {
                return "";
            }

            return tokenArrays[1];
        }

        // 取路由
        public string GetRouterPath()
        {
            var path = (Context.Request.Path.Value ?? "").ToLower();
            return path;
        }

        // 取客户端IP
        public string GetUserIP()
        {
            var ip = Context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = Context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        // http输出
        public void OutHttpResult(Message msg)
        {
            Context.Response.WriteAsJsonAsync(msg);
        }

        #endregion
    }
}
