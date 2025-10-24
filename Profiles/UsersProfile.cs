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
                .ForMember(d => d.PasswordHash, o => o.Ignore())   // set in service
                .ForMember(d => d.CreatedAt, o => o.Ignore())   // set in service
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Trim().ToLower()));

            CreateMap<User, UserReadDto>();

            CreateMap<UserUpdateDto, User>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.Email, o => o.MapFrom((s, d) =>
                    s.Email is null ? d.Email : s.Email.Trim().ToLower()))
                .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

            // No map for LoginDto -> User; login is a verification flow, not a create/update
        }
    }
}
