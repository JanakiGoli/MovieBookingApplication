using AccessIdentityAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccessIdentityAPI.Models
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        { 
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }

        /// <summary>
        /// Creates a JWT token for a user with specified claims and roles, and returns the token string.
        /// </summary>
        /// <param name="appUser">The user for whom the token is being created.</param>
        /// <returns>The JWT token string.</returns>

        public async Task<string> CreateToken(ApplicationUser appUser)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName,appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email,appUser.Email)
            };

            var roles = await _userManager.GetRolesAsync(appUser);
            claims.AddRange(roles.Select(role=> new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = creds,
                Issuer = "Priya",
                Audience = "Priya"
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
