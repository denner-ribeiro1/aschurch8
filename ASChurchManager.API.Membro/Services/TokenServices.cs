using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ASChurchManager.API.Membro.Infra;
using Microsoft.IdentityModel.Tokens;

namespace ASChurchManager.API.Membro.Services;

public class TokenServices
{
    public string Generate(ASChurchManager.Domain.Entities.Membro membro)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Configuration.PrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(membro),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = credentials,
        };
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);

    }
    private static ClaimsIdentity GenerateClaims(ASChurchManager.Domain.Entities.Membro membro)
    {
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim(ClaimTypes.Sid, membro.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Name, membro.Nome));
        ci.AddClaim(new Claim(ClaimTypes.Email, membro.Email));
        ci.AddClaim(new Claim(ClaimTypes.Locality, membro.Congregacao.Nome));

        return ci;
    }
}
