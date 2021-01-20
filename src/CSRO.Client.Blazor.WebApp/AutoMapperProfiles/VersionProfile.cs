using AutoMapper;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Dtos.AzureDtos;
using Models = CSRO.Client.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CSRO.Client.Blazor.WebApp.AutoMapperProfiles
{
    public class VersionProfile : Profile
    {        
        public IMapper Mapper { get; }

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
                //.ForMember(s => s.CurrentUnits, op => op.Ignore())
                .ReverseMap();

            //CreateMap<Models.ResourceGroup, Models.ResourceGroupModel>()
            //    .ForMember(s => s.ResourceGroup, op => op.MapFrom(ss => ss))
            //    .ReverseMap();

            //CreateMap<Models.ResourceGroupModel, Models.ResourceGroup>()                
            //    .ForMember(s => s.Id, op => op.MapFrom(ss => ss.ResourceGroup.Id))
            //    .ForMember(s => s.Location, op => op.MapFrom(ss => ss.ResourceGroup.Location))
            //    .ForMember(s => s.Name, op => op.MapFrom(ss => ss.ResourceGroup.Name))
            //    .ForMember(s => s.Properties, op => op.MapFrom(ss => ss.ResourceGroup.Properties))
            //    .ForMember(s => s.Tags, op => op.MapFrom(ss => ss.ResourceGroup.Tags))
            //    .ForMember(s => s.Type, op => op.MapFrom(ss => ss.ResourceGroup.Type))                
            //    .ReverseMap();

        }        
    }
}
