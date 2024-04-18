using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Snai.CMS.Api_Core.Entities.Settings;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Jwt
{
    public static class JwtDI
    {
        public static IServiceCollection AddJwt(this IServiceCollection services)
        {
            IServiceProvider provider = services.BuildServiceProvider();
            var jwtHelper = provider.GetRequiredService<JwtHelper>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = jwtHelper.TokenValidationParameters;
                options.Events = jwtHelper.JwtBearerEvents;
            });

            return services;
        }
    }
}
