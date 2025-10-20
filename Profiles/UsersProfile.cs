using BookStoreEcommerce.Dtos.User;
using BookStoreEcommerce.Models;

namespace BookStoreEcommerce.Profiles
{
    public class UsersProfile : AutoMapper.Profile
    {
        public UsersProfile()
        {
            // Source -> Target
            CreateMap<UserRegisterDto, User>()
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore());


            CreateMap<User, UserReadDto>();

        }
    }
}