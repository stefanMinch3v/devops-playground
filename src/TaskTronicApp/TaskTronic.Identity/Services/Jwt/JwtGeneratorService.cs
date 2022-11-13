namespace TaskTronic.Identity.Services.Jwt
{
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    public class JwtGeneratorService : IJwtGeneratorService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationSettings applicationSettings;

        public JwtGeneratorService(
            UserManager<ApplicationUser> userManager,
            IOptions<ApplicationSettings> applicationSettings)
        {
            this.userManager = userManager;
            this.applicationSettings = applicationSettings.Value;
        }

        public async Task<OutputJwtModel> GenerateTokenAsync(ApplicationUser appUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.applicationSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                new Claim(ClaimTypes.Name, appUser.Email)
            };

            var roles = await this.userManager.GetRolesAsync(appUser);
            foreach (var roleName in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = tokenHandler.WriteToken(token);

            return new OutputJwtModel(encryptedToken, roles, token.ValidTo);
        }
    }
}
