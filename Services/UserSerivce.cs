using AutoMapper;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Dtos.User;
using BookStoreEcommerce.Models;
using BookStoreEcommerce.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BookStoreEcommerce.Services;


public class UserService(
    StoreDbContext _context,
    IUserRepo _users,
    IMapper _mapper,
    IPasswordHasher<User> _hasher,
    ITokenService _tokens,
    IConfiguration _config) : IUserService
{

    private readonly StoreDbContext _context = _context;
    private readonly IUserRepo _users = _users;
    private readonly IMapper _mapper = _mapper;

    private readonly int _accessMinutes = _config.GetValue<int?>("Jwt:AccessTokenExpireMinutes") ?? 60;


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

        //if email changed, ensure uniqueness
        if (!string.IsNullOrWhiteSpace(userDto.Email) && userDto.Email != existingUser.Email)
        {
            if (_context.Users.Any(u => u.Email == userDto.Email))
                throw new ArgumentException("Email is already registered.");
        }

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
        if (_context.Users.Any(u => u.Email == userRegisterDto.Email))
            throw new ArgumentException("Email is already registered.");

        var user = _mapper.Map<User>(userRegisterDto);
        user.PasswordHash = _hasher.HashPassword(user, userRegisterDto.Password);
        var createdUser = _users.AddUser(user);
        _context.SaveChanges();
        return _mapper.Map<UserReadDto>(createdUser);
    }

    public AuthResponseDto AuthenticateUser(UserLoginDto userLoginDto)
    {
        var normalizedEmail = userLoginDto.Email.Trim().ToLower();
        userLoginDto.Email = normalizedEmail;
        var user = _context.Users.FirstOrDefault(u => u.Email == userLoginDto.Email)
            ?? throw new InvalidOperationException("Invalid credentials");

        var verificationResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, userLoginDto.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
            throw new InvalidOperationException("Invalid credentials");

        var token = _tokens.CreateToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_accessMinutes);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserReadDto>(user)
        };
    }


}