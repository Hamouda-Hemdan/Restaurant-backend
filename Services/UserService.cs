using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using resturant1.Models;
using resturant1.Models.Dto;
using resturant1.Models.Entities; 
using resturant1.Data; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace resturant1.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context; // Added for basket creation

        public UserService(UserManager<User> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        // Register user
        public async Task<IdentityResult> RegisterUserAsync(UserRegisterModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                BirthDate = (DateTime)model.BirthDate,
                Address = model.Address,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Create a basket for the user
                var basket = new Basket
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Items = new List<BasketItem>()
                };

                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync(); // Save basket to the database
            }

            return result;
        }
           // Login user and generate JWT token
           public async Task<string> LoginUserAsync(LoginDto loginDto)
           {
               var user = await _userManager.FindByEmailAsync(loginDto.Email);
               if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
               {
                   return null;
               }
        
               return GenerateJwtToken(user);
           }
         private string GenerateJwtToken(User user)
         {
         var jwtSettings = _configuration.GetSection("JwtSettings");
         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
         var claims = new[]
         {
             new Claim(JwtRegisteredClaimNames.Sub, user.Email),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
             new Claim(ClaimTypes.Email, user.Email)
         };
    
         var token = new JwtSecurityToken(
             issuer: jwtSettings["Issuer"],
             audience: jwtSettings["Audience"],
             claims: claims,
             expires: DateTime.UtcNow.AddSeconds(Convert.ToInt32(jwtSettings["ExpiryInSeconds"])),
             signingCredentials: creds
         );
    
         return new JwtSecurityTokenHandler().WriteToken(token);
         }
    }
}
