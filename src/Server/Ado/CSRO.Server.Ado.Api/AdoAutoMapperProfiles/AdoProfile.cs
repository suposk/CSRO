using AutoMapper;
using Models = CSRO.Server.Ado.Api.Models;
using Entity = CSRO.Server.Entities.Entity;
using AdoModels = CSRO.Common.AdoServices.Models;
using AdoDtos = CSRO.Common.AdoServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CSRO.Server.Ado.Api.AdoAutoMapperProfiles
{
    public class AdoProfile : Profile
    {
        public IMapper Mapper { get; }

        public AdoProfile()
        {
            CreateMap<AdoModels.ProjectAdo, Entity.AdoProject>()                
                .ReverseMap();


            CreateMap<Microsoft.TeamFoundation.Core.WebApi.TeamProject, AdoModels.ProjectAdo>()
                .ForMember(s => s.AdoId, op => op.MapFrom(ss => ss.Id)) //Id is guid                
                .ForMember(s => s.Id, op => op.Ignore())
                .ForMember(s => s.Organization, op => op.Ignore())
                .ForMember(s => s.ProcessName, op => op.Ignore())
                .ReverseMap();

            CreateMap<Microsoft.TeamFoundation.Core.WebApi.Process, AdoModels.ProcessAdo>()
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
