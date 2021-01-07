using AutoMapper;
using CSRO.Server.Domain;
using CSRO.Server.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Api.AutoMapperProfiles
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

            CreateMap<VmTicketDto, VmTicket>()
                .ReverseMap();

        }
    }
}
