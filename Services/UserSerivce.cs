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

    private readonly int _accessMinutes = _config.GetValue<int?>("Jwt:AccessTokenDurationInMinutes") ?? 60;
    private readonly int _refreshDays = _config.GetValue<int?>("Jwt:RefreshTokenDurationInDays") ?? 7;


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

        if (string.IsNullOrEmpty(user.PasswordHash))
            throw new InvalidOperationException("User password not set");
        var verificationResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, userLoginDto.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
            throw new InvalidOperationException("Invalid credentials");

        var accessToken = _tokens.CreateToken(user);
        var accessExpiresAt = DateTime.UtcNow.AddMinutes(_accessMinutes);

        // Issue new refresh token (rotation on every login)
        var newRefreshToken = _tokens.GenerateRefreshToken();
        user.RefreshTokenHash = _tokens.HashToken(newRefreshToken);
        user.RefreshTokenExpiresUtc = DateTime.UtcNow.AddDays(_refreshDays);
        user.RefreshTokenRevokedUtc = null; // reset any previous revoke
        _context.SaveChanges();

        return new AuthResponseDto
        {
            Token = accessToken,
            ExpiresAt = accessExpiresAt,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAt = user.RefreshTokenExpiresUtc,
            User = _mapper.Map<UserReadDto>(user)
        };
    }

    public AuthResponseDto Refresh(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token missing");

        var refreshHash = _tokens.HashToken(refreshToken);
        var user = _context.Users.FirstOrDefault(u => u.RefreshTokenHash == refreshHash);
        if (user == null)
            throw new InvalidOperationException("Invalid refresh token");
        if (user.RefreshTokenRevokedUtc.HasValue)
            throw new InvalidOperationException("Refresh token revoked");
        if (!user.RefreshTokenExpiresUtc.HasValue || user.RefreshTokenExpiresUtc.Value < DateTime.UtcNow)
            throw new InvalidOperationException("Refresh token expired");

        // Rotate refresh token
        var newRefreshToken = _tokens.GenerateRefreshToken();
        user.RefreshTokenHash = _tokens.HashToken(newRefreshToken);
        user.RefreshTokenExpiresUtc = DateTime.UtcNow.AddDays(_refreshDays);
        _context.SaveChanges();

        var newAccessToken = _tokens.CreateToken(user);
        var accessExpiresAt = DateTime.UtcNow.AddMinutes(_accessMinutes);

        return new AuthResponseDto
        {
            Token = newAccessToken,
            ExpiresAt = accessExpiresAt,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiresAt = user.RefreshTokenExpiresUtc,
            User = _mapper.Map<UserReadDto>(user)
        };
    }

    public void RevokeRefreshToken(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId)
            ?? throw new InvalidOperationException("User not found");
        user.RefreshTokenRevokedUtc = DateTime.UtcNow;
        _context.SaveChanges();
    }


}