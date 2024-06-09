// -----------------------------------------------------------------------
// <copyright file="JwtHandler.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.IdentityModel.Tokens;

namespace Diary_Sample.Infra;

public class JwtHandler : IJwtHandler
{
    private readonly JwtConfigurableOptions _jwtConfigurableOptions;

    public JwtHandler(JwtConfigurableOptions jwtConfigurableOptions)
    {
        _jwtConfigurableOptions = jwtConfigurableOptions;
    }

    public string GenerateEncodedToken(string userName, string deviceId, IList<string> roles)
    {
        var jwtConfig = JwtConfigurableOptions.getJwtConfigurableOptions(_jwtConfigurableOptions);
        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTime.UtcNow.AddDays(jwtConfig.JwtExpireDays).Ticks.ToString(), ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.System, deviceId)
        };

        if (roles.Any())
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        // Create the JWT security token and encode it.
        JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: jwtConfig.JwtIssuer,
            audience: jwtConfig.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(jwtConfig.JwtExpireDays),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.JwtKey)),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}