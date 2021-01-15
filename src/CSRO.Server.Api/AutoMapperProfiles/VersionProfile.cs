using AutoMapper;
using CSRO.Server.Domain;
using CSRO.Server.Entities.Entity;
using Entity = CSRO.Server.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRO.Server.Domain.AzureDtos;

namespace CSRO.Server.Api.AutoMapperProfiles
{
    public class VersionProfile : Profile
    {
        public VersionProfile()
        {
            CreateMap<AppVersionDto, Entity.AppVersion>()
                //.ForMember(s => s.CurrentUnits, op => op.Ignore())
                .ReverseMap();

            CreateMap<TicketDto, Entity.Ticket>()
                //.ForMember(s => s.CurrentUnits, op => op.Ignore())
                .ReverseMap();

            CreateMap<VmTicketDto, Entity.VmTicket>()
                //.ForMember(s => s.SubscripionIdName, op => op.Ignore())
                .ReverseMap();

            CreateMap<SubscriptionDto, Subscription>()
                .ReverseMap();

            CreateMap<PropertiesDto, Properties>()
                .ReverseMap();

            CreateMap<ResourceGroupDto, ResourceGroup>()
                .ReverseMap();

        }
    }
}
