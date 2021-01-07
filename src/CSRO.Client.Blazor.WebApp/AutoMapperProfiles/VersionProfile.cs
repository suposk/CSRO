using AutoMapper;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Client.Blazor.WebApp.AutoMapperProfiles
{
    public class VersionProfile : Profile
    {
        public VersionProfile()
        {
            CreateMap<AppVersionDto, AppVersion>()
                //.ForMember(s => s.CurrentUnits, op => op.Ignore())
                .ReverseMap();

            CreateMap<TicketDto, Ticket>()
                //.ForMember(s => s.CurrentUnits, op => op.Ignore())
                .ReverseMap();

            CreateMap<VmDto, Vm>()
                .ReverseMap();
        }
    }
}
