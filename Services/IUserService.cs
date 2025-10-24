using BookStoreEcommerce.Dtos.User;

namespace BookStoreEcommerce.Services;

public interface IUserService
{
    IEnumerable<UserReadDto> GetAllUsers();
    UserReadDto? GetUserById(int id);
    UserReadDto? UpdateUser(int id, UserUpdateDto userDto);
    UserReadDto? DeleteUser(int id);
    // UserReadDto? AuthenticateUser(UserLoginDto userLoginDto);
    UserReadDto RegisterUser(UserRegisterDto userRegisterDto);

}