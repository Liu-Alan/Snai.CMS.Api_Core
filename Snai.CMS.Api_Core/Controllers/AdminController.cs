using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Snai.CMS.Api_Core.Business;
using Snai.CMS.Api_Core.Common.Infrastructure.Extension;
using Snai.CMS.Api_Core.Common.Infrastructure.Jwt;
using Snai.CMS.Api_Core.Common.Infrastructure;
using Snai.CMS.Api_Core.Entities.Settings;
using Microsoft.AspNetCore.Authorization;
using Snai.CMS.Api_Core.Common.Encrypt;
using Snai.CMS.Api_Core.Models;
using System.Net.Http;

namespace Snai.CMS.Api_Core.Controllers
{
    public class AdminController : BaseController
    {
        #region 构造函数

        private readonly ILogger<HomeController> _logger;
        IOptions<WebSettings> _webSettings;
        Consts _consts;
        CMSBO _cmsBO;

        public AdminController(ILogger<HomeController> logger, HttpContextExtension httpContext, IOptions<WebSettings> webSettings, JwtHelper jwtHelper, CMSBO cmsBO, Consts consts)
        {
            _logger = logger;
            _webSettings = webSettings;
            _consts = consts;
            _cmsBO = cmsBO;
        }

        #endregion

        #region 取用户数

        [HttpPost]
        [Authorize(Policy = "Permission")]
        public Message List([FromForm] PageIn pageIn)
        {
            var msg = new Message() { Code = (int)Code.Success, Msg = _consts.GetMsg(Code.Success) };

            var quiryTitle = pageIn.QuiryTitle ?? "";
            var page = pageIn.Page ?? 1;
            var pageSize = pageIn.PageSize ?? _webSettings.Value.DefaultPageSize;

            var admins = _cmsBO.GetAdminList(quiryTitle, page, pageSize);
            if (admins == null || admins.Count <= 0)
            {
                msg.Code = (int)Code.RecordNotFound;
                msg.Msg = _consts.GetMsg(Code.RecordNotFound);
                return msg;
            }

            msg.Result.Data = admins;
            return msg;
        }
    
        #endregion
    }
}
