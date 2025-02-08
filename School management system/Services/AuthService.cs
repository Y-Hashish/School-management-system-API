using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using School_management_system.Helpers;
using School_management_system.Models;

namespace School_management_system.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt)
        {
            _jwt = jwt.Value;
            _userManager = userManager;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new AuthModel { Message = "Email is already registered !" };
            if (await _userManager.FindByNameAsync(model.Email) != null)
                return new AuthModel { Message = "Username is already registered !" };
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, $"{SD.Student}");
            //var token = await CreateTokenAsync(user);
            return new AuthModel
            {
                Email = user.Email,
                UserName = user.UserName,
                //ExpiresOn = token.ValidTo,
                IsAuth = true,
                Roles = new List<string> { $"{SD.Student}" },
                //Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
        public async Task<AuthModel> GenerateToken(LoginModel model)
        {
            var authmodel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authmodel.Message = "Email or Password is incorrect!";
                return authmodel;
            }
            var token = await CreateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            authmodel.ExpiresOn = token.ValidTo;
            authmodel.IsAuth = true;
            authmodel.Email = user.Email;
            authmodel.UserName = user.UserName;
            authmodel.Roles = roles.ToList();
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return authmodel;
        }

        public async Task<JwtSecurityToken> CreateTokenAsync(ApplicationUser user)
        {
            var UserClaims = await _userManager.GetClaimsAsync(user);
            var Claims = new List<Claim>();
            var Roles = await _userManager.GetRolesAsync(user);

            // assign the roles of that user 
            foreach (var item in Roles)
                Claims.Add(new Claim("role", item));

            //add claims 
            Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.UserName));
            Claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            Claims.Add(new Claim("uid", user.Id));


            //generate the key 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //generate token 
            var JwtToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
            audience: _jwt.Audience,
                signingCredentials: signingCredential,
                claims: Claims,
                expires: DateTime.Now.AddDays(_jwt.DaurationInDays)
                );
            return JwtToken;

        }
    }
}
