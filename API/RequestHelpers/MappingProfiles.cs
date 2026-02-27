using API.DTOs;
using AutoMapper;
using Core.Entities;

namespace API.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateAnnouncementDto, Announcement>();
        
        CreateMap<Announcement, AnnouncementDto>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
            .ForMember(d => d.Message, o => o.MapFrom(s => s.Message))
            .ForMember(d => d.ScheduledAt, o => o.MapFrom(s => s.ScheduledAt))
            .ForMember(d => d.ExpireAt, o => o.MapFrom(s => s.ExpireAt))
            .ForMember(d => d.AudioUrl, o => o.MapFrom(s => s.AudioUrl));

        CreateMap<CertificateResponseDto, CreateCertificateDto>();
        CreateMap<BarangayCertificate, CertificateResponseDto>();
        CreateMap<CreateOfficialDto, BarangayOfficial>();
        CreateMap<UpdateOfficialDto, BarangayOfficial>();
        CreateMap<BarangayOfficial, BarangayOfficialDto>();
        CreateMap<Resident, UserInfoDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.AppUserId))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.AppUser!.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.AppUser!.IdUrl))
            .ForMember(d => d.IdUrl, o => o.MapFrom(s => s.AppUser!.IsIdVerified))
            .ForMember(d => d.Role, o => o.Ignore());

    }
}
