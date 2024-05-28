using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using api.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class AuthService
    {
        public AuthService()
        {

        }

        public string GenerateJwt(UserDto user)
        {
            var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? 
            throw new InvalidOperationException("Jwt key is missing in environment variables");
            var jwtIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") ?? 
            throw new InvalidOperationException("Jwt Issuer is missing in environment variables");
            var jwtAudience = Environment.GetEnvironmentVariable("Jwt__Audience") ?? 
            throw new InvalidOperationException("Jwt Audience is missing in environment variables");

            var key = Encoding.ASCII.GetBytes(jwtKey);
            // var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            // var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Role, user.IsAdmin? "Admin" : "User"),
                new Claim(ClaimTypes.Role, user.IsBanned? "Banned" : "notBanned"),
            }),

                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),

                Issuer = jwtIssuer,
                Audience = jwtAudience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? 
            throw new InvalidOperationException("Jwt key is missing in environment variables");
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}