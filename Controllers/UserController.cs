using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using resturant1.Models.Dto;
using resturant1.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace resturant1.Controllers
{
    [Route("api/account/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

      
        [AllowAnonymous]
        [HttpPost("register")]
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Status = "400",
                    Message = "Bad Request: Invalid model state."
                });
            }

            var result = await _userService.RegisterUserAsync(model);
            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponse
                {
                    Status = "400",
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            return Ok(new SuccessResponse
            {
                Status = "200",
                Message = "Registration successful.",
                
            });
        }
        // Login user and generate JWT token
         [AllowAnonymous]
         [HttpPost("login")]
         public async Task<IActionResult> Login(LoginDto loginDto)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(new ErrorResponse
                 {
                     Status = "400",
                     Message = "Bad Request: Invalid login credentials."
                 });
             }
        
             var token = await _userService.LoginUserAsync(loginDto);
             if (token == null)
             {
                 return Unauthorized(new ErrorResponse
                 {
                     Status = "401",
                     Message = "Unauthorized: Invalid email or password."
                 });
             }
        
             return Ok(new SuccessResponse
             {
                 Status = "200",
                 Message = "Login successful.",
                 Data = new { Token = token }
             });
         }
           public IActionResult Logout()
  {
      // In case of stateless JWT tokens, logging out can be done client-side by removing the token.
      return Ok(new SuccessResponse
      {
          Status = "200",
          Message = "Logged out successfully."
      });
  }
    }
}
