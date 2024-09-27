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
using Snai.CMS.Api_Core.Entities.CMS;

namespace Snai.CMS.Api_Core.Controllers
{
    public class AdminController : BaseController
    {
        #region 构造函数

        private readonly ILogger<AdminController> _logger;
        IOptions<WebSettings> _webSettings;
        Consts _consts;
        CMSBO _cmsBO;

        public AdminController(ILogger<AdminController> logger, HttpContextExtension httpContext, IOptions<WebSettings> webSettings, JwtHelper jwtHelper, CMSBO cmsBO, Consts consts)
        {
            _logger = logger;
            _webSettings = webSettings;
            _consts = consts;
            _cmsBO = cmsBO;
        }

        #endregion

        #region 取用户数

        [HttpGet]
        [Authorize(Policy = "Permission")]
        public Message List([FromQuery] PageIn pageIn)
        {
            var msg = new Message() { Code = (int)Code.Success, Msg = _consts.GetMsg(Code.Success) };

            var quiryTitle = pageIn.QuiryTitle ?? "";
            var page = Convert.ToInt32(pageIn.Page ?? "1");
            var pageSize = Convert.ToInt32(pageIn.PageSize ?? _webSettings.Value.DefaultPageSize.ToString());

            var (admins,pager, pageSizer) = _cmsBO.GetAdminList(quiryTitle, page, pageSize);
            if (admins == null || admins.Count <= 0)
            {
                msg.Code = (int)Code.RecordNotFound;
                msg.Msg = _consts.GetMsg(Code.RecordNotFound);
                return msg;
            }

            var pageResult = new PageOut<Admin>();
            pageResult.PageList = admins;
            pageResult.PageSize = pageSizer;
            pageResult.Page = pager;
            pageResult.Total = _cmsBO.GetAdminCount(quiryTitle);

            msg.Result.Data = pageResult;
            return msg;
        }
    
        #endregion
    }
}
