using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStoreEcommerce.Models;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreEcommerce.Services.Auth;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(User user)
    {
        var jwtSettings = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

        var adminEmails = config.GetSection("Auth:AdminEmails").Get<List<string>>() ?? [];
        var isAdmin = adminEmails.Any(email => email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));


        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
            new Claim(ClaimTypes.Email,  user.Email!)

        };

        if (isAdmin)
        {
            var claimsList = claims.ToList();
            claimsList.Add(new Claim(ClaimTypes.Role, "Admin"));
            claims = claimsList.ToArray();
        }
        else
        {
            var claimsList = claims.ToList();
            claimsList.Add(new Claim(ClaimTypes.Role, "Self"));
            claims = claimsList.ToArray();
        }
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(
            double.Parse(jwtSettings["AccessTokenDurationInMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}