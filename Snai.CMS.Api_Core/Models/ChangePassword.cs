using Microsoft.AspNetCore.Mvc;
using Snai.CMS.Api_Core.Common.Infrastructure.Validation;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Snai.CMS.Api_Core.Models
{
    public class ChangePasswordIn
    {
        [ModelBinder(Name = "old_password")]
        [Required(ErrorMessage = "原密码不能为空"), EnglishNumberCombination(ErrorMessage = "密码须英文加数字组合且6位及以上")]
        public string OldPassword { get; set; }

        [ModelBinder(Name = "password")]
        [Required(ErrorMessage = "新密码不能为空"), EnglishNumberCombination(ErrorMessage = "密码须英文加数字组合且6位及以上")]
        public string Password { get; set; }

        [ModelBinder(Name= "re_password")]
        [Required(ErrorMessage = "确认密码不能为空"), EnglishNumberCombination(ErrorMessage = "密码须英文加数字组合且6位及以上")]
        [Compare("Password", ErrorMessage = "确认密码与新密码须一致")]
        public string RePassword { get; set; }
    }
}
