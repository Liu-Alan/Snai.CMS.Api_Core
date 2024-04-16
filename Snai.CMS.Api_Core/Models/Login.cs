using Snai.CMS.Api_Core.Common.Infrastructure.Validation;
using System.ComponentModel.DataAnnotations;

namespace Snai.CMS.Api_Core.Models
{
    public class LoginIn
    {
        [Required(ErrorMessage = "用户名不能为空"), MaxLength(32, ErrorMessage = "用户名最长32位")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空"), EnglishNumberCombination(ErrorMessage= "密码须英文加数字组合且6位及以上")]
        public string Password { get; set; }
    }

    public class LoginOut
    {
        public string Token { get; set; }
    }
}
