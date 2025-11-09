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

    [HttpPost("refresh")]
    [AllowAnonymous]
    public ActionResult<AuthResponseDto> RefreshToken([FromBody] RefreshRequestDto request)
    {
        try
        {
            var response = userService.Refresh(request.RefreshToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid token: missing user id." });
        userService.RevokeRefreshToken(userId);
        return NoContent();
    }
}