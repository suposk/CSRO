using AutoMapper;
using CSRO.Server.Entities.Entity;
using Entity = CSRO.Server.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRO.Server.Domain.AzureDtos;
using CSRO.Server.Domain;
using CSRO.Server.Services.Models;

namespace CSRO.Server.Api.AutoMapperProfiles
{
    public class VersionProfile : Profile
    {
        public VersionProfile()
        {
            CreateMap<Dtos.AppVersionDto, Entity.AppVersion>()
                //.ForMember(s => s.CurrentUnits, op => op.Ignore())
                .ReverseMap();

            CreateMap<TicketDto, Entity.Ticket>()                
                .ReverseMap();

            CreateMap<VmTicketDto, Entity.VmTicket>()                
                .ReverseMap();

            CreateMap<VmTicketHistoryDto, Entity.VmTicketHistory>()                
                .ReverseMap();

            CreateMap<SubscriptionDto, Subscription>()
                .ReverseMap();

            CreateMap<PropertiesDto, Properties>()
                .ReverseMap();

            CreateMap<ResourceGroupDto, ResourceGroup>()
                .ReverseMap();

            CreateMap<IdNameDto, IdName>()
                .ReverseMap();
                        
            CreateMap<CustomerDto, Entity.ResourceSWI>()
                .ForMember(s => s.Id, op => op.Ignore())
                .ReverseMap();
                        
            CreateMap<cmdbReferenceDto, cmdbReferenceModel>()
                .ReverseMap();

            CreateMap<opEnvironmentDto, opEnvironmentModel>()
                .ReverseMap();

            
        }
    }
}
