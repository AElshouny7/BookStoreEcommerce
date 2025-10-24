using AutoMapper;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Dtos.User;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Services;


public class UserService(
    StoreDbContext _context,
    IUserRepo _users,
    IMapper _mapper) : IUserService
{

    private readonly StoreDbContext _context = _context;
    private readonly IUserRepo _users = _users;
    private readonly IMapper _mapper = _mapper;

    public IEnumerable<UserReadDto> GetAllUsers()
    {
        var users = _users.GetAllUsers();
        return _mapper.Map<IEnumerable<UserReadDto>>(users);
    }

    public UserReadDto? GetUserById(int id)
    {
        var user = _users.GetUserById(id) ?? throw new InvalidOperationException("User not found");
        return _mapper.Map<UserReadDto>(user);
    }

    public UserReadDto? UpdateUser(int id, UserUpdateDto userDto)
    {
        var existingUser = _users.GetUserById(id) ?? throw new InvalidOperationException("User not found");
        _mapper.Map(userDto, existingUser);
        var updatedUser = _users.UpdateUser(existingUser);
        _context.SaveChanges();

        return _mapper.Map<UserReadDto>(updatedUser);
    }

    public UserReadDto? DeleteUser(int id)
    {
        var user = _users.GetUserById(id)
        ?? throw new InvalidOperationException("User not found");
        var deletedUser = _users.DeleteUser(id);
        _context.SaveChanges();
        return _mapper.Map<UserReadDto>(deletedUser);
    }

    // public UserReadDto? AuthenticateUser(UserLoginDto userLoginDto)
    // {
    //     var user = _users.AuthenticateUser(userLoginDto) ?? throw new InvalidOperationException("Invalid credentials");
    //     return _mapper.Map<UserReadDto>(user);
    // }

    public UserReadDto RegisterUser(UserRegisterDto userRegisterDto)
    {
        var user = _mapper.Map<User>(userRegisterDto);
        var createdUser = _users.AddUser(user);
        _context.SaveChanges();
        return _mapper.Map<UserReadDto>(createdUser);
    }



}