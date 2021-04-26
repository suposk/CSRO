﻿using AutoMapper;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Dtos.AzureDtos;
using Models = CSRO.Client.Services.Models;
using AdoModels = CSRO.Common.AdoServices.Models;
using SdkModels = CSRO.Common.AzureSdkServices.Models;
using AdoDtos = CSRO.Common.AdoServices.Dtos;
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
                .ReverseMap();

            CreateMap<SdkModels.IdNameSdk, Core.Models.IdName>()
                .ForMember(s => s.Id, op => op.MapFrom(ss => ss.Id))
                .ForMember(s => s.Name, op => op.MapFrom(ss => ss.Name))                                
                .ReverseMap();
                        
            CreateMap<IdNameDto, Core.Models.IdName>()
                .ReverseMap();

            CreateMap<TicketDto, Models.Ticket>()
                .ForMember(s => s.CreatedBy, op => op.MapFrom(ss => ss.CreatedBy.Replace("live.com#", string.Empty)))
                .ReverseMap();

            CreateMap<VmTicketDto, Models.VmTicket>()
                .ForMember(s => s.SubscripionIdName, op => op.Ignore())
                .ReverseMap();

            CreateMap<VmTicketHistoryDto, Models.VmTicketHistory>()                
                .ReverseMap();

            CreateMap<SubscriptionDto, Models.Subscription>()
                .ReverseMap();

            CreateMap<Models.Subscription, Azure.ResourceManager.Resources.Models.Subscription>()
                .ForMember(s => s.AuthorizationSource, op => op.Ignore())
                //.ForMember(s => s.TenantId, op => op.Ignore())
                .ForMember(s => s.ManagedByTenants, op => op.Ignore())
                .ForMember(s => s.SubscriptionPolicies, op => op.Ignore())
                .ReverseMap();                      

            CreateMap<PropertiesDto, Models.Properties>()
                .ReverseMap();

            CreateMap<CreateRgTagDto, Models.DefaultTag>()
                .ForMember(s => s.privilegedMembers, op => op.Ignore())
                .ReverseMap();

            CreateMap<TagDto, Models.DefaultTag>()
                .ReverseMap();

            CreateMap<ResourceGroupDto, Models.ResourceGroup>()                
                .ReverseMap();

            CreateMap<ResourceGroupCreateDto, Models.ResourceGroup>()
                .ForMember(s => s.IsPrivMembersRequired, op => op.Ignore())                
                .ReverseMap();

            CreateMap<Microsoft.TeamFoundation.Core.WebApi.TeamProject, AdoModels.ProjectAdo>()
                .ForMember(s => s.AdoId, op => op.MapFrom(ss => ss.Id)) //Id is guid
                .ForMember(s => s.Id, op => op.Ignore())
                .ForMember(s => s.Organization, op => op.Ignore())
                .ForMember(s => s.ProcessName, op => op.Ignore())                
                .ReverseMap();

            CreateMap<Microsoft.TeamFoundation.Core.WebApi.Process, AdoModels.ProcessAdo>()                
                .ReverseMap();

            CreateMap<AdoProjectHistoryDto, Models.AdoProjectHistoryModel>()
                //.ForMember(s => s.AdoProject, op => op.Ignore())
                .ReverseMap();

            CreateMap<CustomerDto, Models.Customer>()
                .ForMember(s => s.ShowDetails, op => op.Ignore())
                .ReverseMap();

            CreateMap<cmdbReferenceDto, Models.cmdbReference>()
                .ReverseMap();

            CreateMap<opEnvironmentDto, Models.opEnvironment>()
                .ReverseMap();

            CreateMap<AdoProjectAccessDto, Models.AdoProjectAccessModel>()
                .ReverseMap();

            CreateMap<RoleDto, Models.Role>()
                .ForMember(s => s.CreatedAt, op => op.Ignore())
                .ForMember(s => s.CreatedBy, op => op.Ignore())
                .ForMember(s => s.ModifiedAt, op => op.Ignore())
                .ForMember(s => s.ModifiedBy, op => op.Ignore())
                .ForMember(s => s.RowVersion, op => op.Ignore())
                .ForMember(s => s.Id, op => op.MapFrom(ss => ss.RoleId)) //Id is guid      
                .ForMember(s => s.Name, op => op.MapFrom(ss => ss.Name))
                .ReverseMap();

            CreateMap<UserRoleDto, Models.UserRole>() 
                .ReverseMap();

            CreateMap<UserDto, Models.User>()
                .ForMember(s => s.CreatedAt, op => op.Ignore())
                .ForMember(s => s.CreatedBy, op => op.Ignore())
                .ForMember(s => s.ModifiedAt, op => op.Ignore())
                .ForMember(s => s.ModifiedBy, op => op.Ignore())
                .ForMember(s => s.RowVersion, op => op.Ignore())
                //.ForMember(s => s.UserClaims, op => op.Ignore())
                .ReverseMap();

            CreateMap<UserClaimDto, Models.UserClaim>()
                .ForMember(s => s.Value, op => op.MapFrom(ss => ss.Value))
                .ForMember(s => s.Type, op => op.MapFrom(ss => ss.Type))
                .ForMember(s => s.CreatedAt, op => op.Ignore())
                .ForMember(s => s.CreatedBy, op => op.Ignore())
                .ForMember(s => s.ModifiedAt, op => op.Ignore())
                .ForMember(s => s.ModifiedBy, op => op.Ignore())
                .ForMember(s => s.RowVersion, op => op.Ignore())
                .ForMember(s => s.Id, op => op.Ignore())//base
                .ForMember(s => s.User, op => op.Ignore())
                .ForMember(s => s.UserName, op => op.Ignore())
                .ReverseMap();

        }        
    }
}
