using BookStoreEcommerce.Dtos.User;
using BookStoreEcommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreEcommerce.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    // GET all users
    [HttpGet]
    public ActionResult<IEnumerable<UserReadDto>> GetAllUsers()
    {
        var users = _userService.GetAllUsers();
        return Ok(users);
    }

    // GET user by id
    [HttpGet("{id:int}", Name = "GetUserById")]
    public ActionResult<UserReadDto> GetUserById(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // // POST create new user
    // [HttpPost]
    // public ActionResult<UserReadDto> AddUser(UserCreateDto userCreateDto)
    // {
    //     try
    //     {
    //         var createdUser = _userService.AddUser(userCreateDto);

    //         return CreatedAtRoute(nameof(GetUserById), new { Id = createdUser.Id }, createdUser);
    //     }
    //     catch (ArgumentException ex)
    //     {
    //         return BadRequest(new { message = ex.Message });
    //     }
    // }

    // PUT update user
    [HttpPut("{id}")]
    public ActionResult<UserReadDto> UpdateUser(int id, UserUpdateDto userUpdateDto)
    {
        try
        {
            var updatedUser = _userService.UpdateUser(id, userUpdateDto);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE user
    [HttpDelete("{id}")]
    public ActionResult<UserReadDto> DeleteUser(int id)
    {
        try
        {
            var deletedUser = _userService.DeleteUser(id);
            if (deletedUser == null)
            {
                return NotFound();
            }
            return Ok(deletedUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}