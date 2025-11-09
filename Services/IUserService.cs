using BookStoreEcommerce.Dtos.User;

namespace BookStoreEcommerce.Services;

public interface IUserService
{
    IEnumerable<UserReadDto> GetAllUsers();
    UserReadDto? GetUserById(int id);
    UserReadDto? UpdateUser(int id, UserUpdateDto userDto);
    UserReadDto? DeleteUser(int id);
    UserReadDto RegisterUser(UserRegisterDto userRegisterDto);

    AuthResponseDto AuthenticateUser(UserLoginDto userLoginDto);

    AuthResponseDto Refresh(string refreshToken);
    void RevokeRefreshToken(int userId);

}