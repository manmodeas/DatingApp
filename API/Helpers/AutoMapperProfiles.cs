using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        //Important Note : If you get auto mapper error then don't try to solve it from client side 
        //Check the terminal of serve side there it will have proper detailed explaination 
        //why did the auto mapper failed 
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
                .ForMember(d => d.PhotoUrl, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));
            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));

            CreateMap<Message, MessageDto>()
                .ForMember(x => x.SenderPhotoUrl, 
                    o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
                .ForMember(x => x.RecipientPhotoUrl, 
                    o => o.MapFrom(m => m.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        }
    }
}
