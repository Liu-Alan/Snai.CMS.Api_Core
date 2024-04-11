namespace Snai.CMS.Api_Core.Entities.Settings
{
    public class JwtSettings
    {
        //密钥
        public string Secret { get; set; }

        //颁发者
        public string Issuer { get; set; }

        //过期时间（秒）
        public int Expire { get; set; }
    }
}
