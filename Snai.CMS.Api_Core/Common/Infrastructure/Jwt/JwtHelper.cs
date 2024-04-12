using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Snai.CMS.Api_Core.Entities.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Jwt
{
    public class JwtHelper
    {
        IOptions<JwtSettings> _jwtSettings;

        public JwtHelper(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        //验证token
        public TokenValidationParameters TokenValidationParameters => new TokenValidationParameters
        {
            ValidateIssuer = true,                      //是否验证Issuer
            ValidateAudience = false,                   //是否验证Audience
            ValidateIssuerSigningKey = true,            //是否验证SecurityKey
            ValidateLifetime = true,                    //是否验证失效时间
            ValidIssuer = _jwtSettings.Value.Issuer,    //发行人Issuer
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret))      //SecurityKey
        };

        // 失败返回
        public JwtBearerEvents JwtBearerEvents => new JwtBearerEvents
        {
            //权限验证失败后执行
            OnChallenge = context =>
            {
                //终止默认的返回结果(必须有)
                context.HandleResponse();
                var result = JsonConvert.SerializeObject(new { Code = "401", Message = "验证失败" });
                context.Response.ContentType = "application/json";
                //验证失败返回401
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.WriteAsync(result);
                return Task.FromResult(0);
            }
        };

        // 生成token
        public string GenerateJwtToken(string userName)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret));
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,          //Issuer
                claims: claims,                             //Claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(_jwtSettings.Value.Expire),       //Expires
                signingCredentials: signingCredentials      //Credentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }

        public class JwtUserInfo
        {
            public string? UserName { get; set; }
        }

        /// <summary>
        /// 将JWT加密的字符串进行解析
        /// </summary>
        /// <param name="jwtStr">JWT加密的字符</param>
        /// <returns>JWT中的用户信息</returns>
        public JwtUserInfo SerializeJwtStr(string jwtStr)
        {
            JwtUserInfo jwtUserInfo = new JwtUserInfo();
            var jwtHandler = new JwtSecurityTokenHandler();

            if (!string.IsNullOrEmpty(jwtStr) && jwtHandler.CanReadToken(jwtStr))
            {
                //将JWT字符读取到JWT对象
                JwtPayload jwtPayload = jwtHandler.ReadJwtToken(jwtStr).Payload;

                //获取JWT中的用户信息
                string? UserName = jwtPayload.Claims.FirstOrDefault(r => r.Type == ClaimTypes.Name)?.Value; 
                jwtUserInfo.UserName = UserName == null ? "" : UserName;
            }

            return jwtUserInfo;
        }
    }
}
