using BusBookingAppln.Models.DBModels;
using BusBookingAppln.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusBookingAppln.Services.Classes
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly SymmetricSecurityKey _key;


        public TokenService(IConfiguration configuration)
        {
            _secretKey = configuration.GetSection("TokenKey").GetSection("JWT").Value.ToString();
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        }
        
        
        // Generate JWT token with Symmetric key
        public string GenerateToken<T>(T user)
        {
            string token = string.Empty;
            var claims = new List<Claim>();

            // Add claims
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null)
            {
                claims.Add(new Claim("ID", idProperty.GetValue(user).ToString()));
            }

            var nameProperty = typeof(T).GetProperty("Name");
            if (nameProperty != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, nameProperty.GetValue(user).ToString()));
            }

            var emailProperty = typeof(T).GetProperty("Email");
            if (emailProperty != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, emailProperty.GetValue(user).ToString()));
            }

            if (typeof(T).Name == "User")
            {
                var roleProperty = typeof(T).GetProperty("Role");
                if (roleProperty != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleProperty.GetValue(user).ToString()));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Driver"));
            }

            // Algorithm
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            // Generate token
            var myToken = new JwtSecurityToken(null, null, claims, expires: DateTime.Now.AddDays(2), signingCredentials: credentials);

            // Convert token to string
            token = new JwtSecurityTokenHandler().WriteToken(myToken);
            return token;
        }
    }
}
