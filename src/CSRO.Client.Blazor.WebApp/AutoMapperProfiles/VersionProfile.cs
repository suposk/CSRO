using AutoMapper;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Dtos.AzureDtos;
using Models = CSRO.Client.Services.Models;
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
            CreateMap<AppVersionDto, Models.AppVersion>()
                //.ForMember(s => s.CurrentUnits, op => op.Ignore())
                .ReverseMap();

            CreateMap<TicketDto, Models.Ticket>()                
                .ReverseMap();

            CreateMap<VmTicketDto, Models.VmTicket>()
                .ForMember(s => s.SubscripionIdName, op => op.Ignore())
                .ReverseMap();

            CreateMap<SubscriptionDto, Models.Subscription>()
                .ReverseMap();

            CreateMap<PropertiesDto, Models.Properties>()
                .ReverseMap();

            CreateMap<TagDto, Models.Tag>()
                .ReverseMap();

            CreateMap<ResourceGroupDto, Models.ResourceGroup>()
                .ReverseMap();
            
            //CreateMap<IdName, SubscriptionsDto>()
            //    .ForMember(s => s., op => op.)
            //    .ReverseMap();
        }
    }
}
