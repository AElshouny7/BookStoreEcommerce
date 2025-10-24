using AutoMapper;
using BookStoreEcommerce.Dtos.User;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserRegisterDto, User>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email!.Trim().ToLower()))
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName!.Trim()));

            CreateMap<User, UserReadDto>();

            CreateMap<UserUpdateDto, User>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                // Only map Email if provided AND not whitespace, and normalize it
                .ForMember(d => d.Email, o =>
                    {
                        o.PreCondition(s => !string.IsNullOrWhiteSpace(s.Email));
                        o.MapFrom(s => s.Email!.Trim().ToLower());
                    })
                // Only map FullName if provided AND not whitespace
                .ForMember(d => d.FullName, o =>
                    {
                        o.PreCondition(s => !string.IsNullOrWhiteSpace(s.FullName));
                        o.MapFrom(s => s.FullName!.Trim());
                    });


            // No map for LoginDto -> User; login is a verification flow, not a create/update
        }
    }
}
