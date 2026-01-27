// using AutoMapper;
// using SmallEcommerceApi.DTOs.Auth;
// using SmallEcommerceApi.Models.Users;


// namespace SmallEcommerceApi.Mapping
// {
//     public class UserMappingProfile : Profile
//     {
//         public UserMappingProfile()
//         {
//             CreateMap<User, UserDto>()
//                 .ForMember(dest => dest.Role,
//                     opt => opt.MapFrom(src => src.Role.RoleName));

//             CreateMap<UpdateUserDto, User>()
//                 .ForMember(dest => dest.UserId, opt => opt.Ignore())
//                 .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
//                 .ForMember(dest => dest.Email, opt => opt.Ignore())
//                 .ForMember(dest => dest.Username, opt => opt.Ignore())
//                 .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
//                 .ForMember(dest => dest.UpdatedAt,
//                     opt => opt.MapFrom(_ => DateTime.UtcNow));
//         }
//     }
// }