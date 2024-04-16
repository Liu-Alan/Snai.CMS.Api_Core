using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Snai.CMS.Api_Core.Business;
using Snai.CMS.Api_Core.Common.Infrastructure;
using Snai.CMS.Api_Core.Common.Infrastructure.Extension;
using Snai.CMS.Api_Core.Common.Infrastructure.Jwt;
using Snai.CMS.Api_Core.Common.Utils;
using Snai.CMS.Api_Core.Entities.CMS;
using Snai.CMS.Api_Core.Models;

namespace Snai.CMS.Api_Core.Controllers
{
    public class HomeController : BaseController
    {
        #region 构造函数

        private readonly ILogger<HomeController> _logger;
        HttpContextExtension _httpContext;
        JwtHelper _jwtHelper;
        CMSBO _cmsBO;
        public HomeController(ILogger<HomeController> logger, HttpContextExtension httpContext, CMSBO cmsBO, JwtHelper jwtHelper)
        {
            _logger = logger;
            _httpContext = httpContext;
            _jwtHelper = jwtHelper;
            _cmsBO = cmsBO;
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return Ok(new { Id = 1, Value = "service run" });
        }

        #region 登录 登出 修改密码

        [HttpPost]
        [AllowAnonymous]
        public Message Login([FromForm] LoginIn loginIn)
        {
            var ip = _httpContext.GetUserIP();
            var (admin, msg) = _cmsBO.AdminLogin(loginIn.UserName, loginIn.Password, ip);
            if (msg.Code == (int)Code.Success && admin != null)
            {
                var token = _jwtHelper.GenerateJwtToken(admin.UserName);
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("JwtHelper.GenerateToken 生成失败");
                    msg.Code = (int)Code.Error;
                    msg.Msg = "登录失败Token01";
                    return msg;
                }
                else
                {
                    var tk = new Token()
                    {
                        TokenStr = token,
                        UserID = admin.ID,
                        State = 1,
                        CreateTime = (int)DateTimeUtils.DateTimeToUnixTimeStamp(DateTime.Now),
                    };
                    msg = _cmsBO.AddToken(tk);
                    if (msg.Code != (int)Code.Success)
                    {
                        _logger.LogError(msg.Msg);
                        msg.Code = (int)Code.Error;
                        msg.Msg = "登录失败Token02";
                        return msg;
                    }
                    else
                    {
                        msg.Result = new LoginOut() { Token = token };
                        return msg;
                    }
                }
            }
            else
            {
                return msg;
            }
        }

        [HttpPost]
        [Authorize(Policy = "Permission")]
        public Message Logout()
        {
            var tokenStr = _httpContext.GetToken();
            if (string.IsNullOrEmpty(tokenStr)) 
            {
                _logger.LogError("取Token失败，请检查Headers.Authorization");
                var msg = new Message() { Code = (int)Code.Fail, Msg = "Token有误，已退出" }; 
                return msg;
            }
            else 
            {
                var tk = _cmsBO.GetToken(tokenStr);
                if (tk == null ||tk.ID<=0)
                {
                    var msg = new Message() { Code = (int)Code.Fail, Msg = "Token有误，已退出" };
                    return msg;
                }
                else
                {
                    tk.State = 2;
                    _cmsBO.ModifyToken(tk);

                    var msg = new Message() { Code = (int)Code.Success, Msg = "已退出" };
                    return msg;
                }
            }
        }

        #endregion  
    }
}
