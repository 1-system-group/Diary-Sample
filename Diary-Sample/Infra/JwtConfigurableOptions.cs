using Microsoft.AspNetCore.Hosting;

namespace Diary_Sample.Infra;

public class JwtConfigurableOptions
{
    public string JwtKey { get; set; }
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
    public int JwtExpireDays { get; set; }

    public static JwtConfigurableOptions getJwtConfigurableOptions(JwtConfigurableOptions jwtConfigurableOptions)
    {
        var jwtExpireDays = Environment.GetEnvironmentVariable("JWT_CONFIGURABLE_OPTIONS_JWT_EXPIRE_DAYS");
        return new JwtConfigurableOptions
        {
            JwtKey = Environment.GetEnvironmentVariable("JWT_CONFIGURABLE_OPTIONS_JWT_KEY") ??
                     jwtConfigurableOptions.JwtKey,
            JwtIssuer = Environment.GetEnvironmentVariable("JWT_CONFIGURABLE_OPTIONS_JWT_ISSUER") ??
                        jwtConfigurableOptions.JwtIssuer,
            JwtAudience = Environment.GetEnvironmentVariable("JWT_CONFIGURABLE_OPTIONS_JWT_AUDIENCE") ??
                          jwtConfigurableOptions.JwtAudience,
            JwtExpireDays = jwtExpireDays != null
                ? Convert.ToInt32(jwtExpireDays)
                : jwtConfigurableOptions.JwtExpireDays
        };
    }
}