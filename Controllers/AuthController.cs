using BookStoreEcommerce.Dtos.User;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService) : ControllerBase
{

    [HttpPost("register")]
    [AllowAnonymous]
    public ActionResult<UserReadDto> RegisterUser(UserRegisterDto userRegisterDto)
    {
        try
        {
            var registeredUser = userService.RegisterUser(userRegisterDto);
            return CreatedAtRoute("GetUserById", new { id = registeredUser.Id }, registeredUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<AuthResponseDto> LoginUser(UserLoginDto userLoginDto)
    {
        try
        {
            var authResponse = userService.AuthenticateUser(userLoginDto);
            return Ok(authResponse);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}