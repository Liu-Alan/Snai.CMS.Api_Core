using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Snai.CMS.Api_Core.Business;
using Snai.CMS.Api_Core.Common.Encrypt;
using Snai.CMS.Api_Core.Common.Infrastructure;
using Snai.CMS.Api_Core.Common.Infrastructure.Extension;
using Snai.CMS.Api_Core.Common.Infrastructure.Jwt;
using Snai.CMS.Api_Core.Common.Utils;
using Snai.CMS.Api_Core.Entities.CMS;
using Snai.CMS.Api_Core.Entities.Settings;
using Snai.CMS.Api_Core.Models;

namespace Snai.CMS.Api_Core.Controllers
{
    public class HomeController : BaseController
    {
        #region 构造函数

        private readonly ILogger<HomeController> _logger;
        HttpContextExtension _httpContext;
        IOptions<PwdSaltSettings> _pwdSaltSettings;
        JwtHelper _jwtHelper;
        Consts _consts;
        CMSBO _cmsBO;
        public HomeController(ILogger<HomeController> logger, HttpContextExtension httpContext, IOptions<PwdSaltSettings> pwdSaltSettings, JwtHelper jwtHelper, CMSBO cmsBO,Consts consts)
        {
            _logger = logger;
            _httpContext = httpContext;
            _pwdSaltSettings = pwdSaltSettings;
            _jwtHelper = jwtHelper;
            _consts = consts;
            _cmsBO = cmsBO;
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return Ok(new Message() { Code = (int)Code.Success, Msg = _consts.GetMsg(Code.Success) });
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
                        msg.Result.Data = new LoginOut() { Token = token };
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

        [HttpPost]
        [Authorize(Policy = "Permission")]
        public Message ChangePassword([FromForm] ChangePasswordIn changePassword)
        {
            var token = _httpContext.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                var msg = new Message() { Code = (int)Code.AuthNotExist, Msg = _consts.GetMsg(Code.AuthNotExist) };
                return msg;
            }

            var userInfo = _jwtHelper.SerializeJwtStr(token);
            if(userInfo == null || string.IsNullOrEmpty(userInfo.UserName))
            {
                var msg = new Message() { Code = (int)Code.AuthCheckFail, Msg = _consts.GetMsg(Code.AuthCheckFail) };
                return msg;
            }

            var admin = _cmsBO.GetAdmin(userInfo.UserName);
            if (admin == null || admin.ID <= 0)
            {
                var msg = new Message() { Code = (int)Code.RecordNotFound, Msg = _consts.GetMsg(Code.RecordNotFound) };
                return msg;
            }

            var pwd = EncryptMd5.EncryptByte(_pwdSaltSettings.Value.Salt + changePassword.OldPassword);
            if (!admin.Password.Equals(pwd, StringComparison.OrdinalIgnoreCase))
            {
                var msg = new Message() { Code = (int)Code.Error, Msg = "原密码错误" };
                return msg;
            }

            admin.Password = EncryptMd5.EncryptByte(_pwdSaltSettings.Value.Salt + changePassword.Password);
            var msgM = _cmsBO.ModifyAdmin(admin);
            if (msgM.Code == (int)Code.Success) 
            {
                var msg = new Message() { Code = (int)Code.Success, Msg = "修改成功" };
                return msg;
            }
            else
            {
                return msgM;
            }
        }

        #endregion  
    }
}
