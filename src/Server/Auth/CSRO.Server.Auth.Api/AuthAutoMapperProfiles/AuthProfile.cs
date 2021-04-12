using AutoMapper;
using Entity = CSRO.Server.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CSRO.Server.Auth.Api.Dtos;

namespace CSRO.Server.Auth.Api.AuthAutoMapperProfiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RoleDto, Entity.Role>()
                .ForMember(s => s.CreatedAt, op => op.Ignore())
                .ForMember(s => s.CreatedBy, op => op.Ignore())
                .ForMember(s => s.ModifiedAt, op => op.Ignore())
                .ForMember(s => s.ModifiedBy, op => op.Ignore())
                .ForMember(s => s.RowVersion, op => op.Ignore())                
                .ForMember(s => s.Id, op => op.MapFrom(ss => ss.RoleId)) //Id is guid      
                .ForMember(s => s.Name, op => op.MapFrom(ss => ss.Name))
                .ReverseMap();

            CreateMap<UserRoleDto, Entity.UserRole>()
                .ForMember(s => s.CreatedAt, op => op.Ignore())
                .ForMember(s => s.CreatedBy, op => op.Ignore())
                .ForMember(s => s.ModifiedAt, op => op.Ignore())
                .ForMember(s => s.ModifiedBy, op => op.Ignore())
                .ForMember(s => s.RowVersion, op => op.Ignore())                                
                //.ForMember(s => s.Role.Id, op => op.MapFrom(ss => ss.RoleId)) //Id is guid      
                //.ForMember(s => s.Role.Name, op => op.MapFrom(ss => ss.Name)) //Id is guid      
                .ReverseMap();

            CreateMap<UserDto, Entity.User>()
                .ForMember(s => s.CreatedAt, op => op.Ignore())
                .ForMember(s => s.CreatedBy, op => op.Ignore())
                .ForMember(s => s.ModifiedAt, op => op.Ignore())
                .ForMember(s => s.ModifiedBy, op => op.Ignore())
                .ForMember(s => s.RowVersion, op => op.Ignore())                                
                .ForMember(s => s.Claims, op => op.Ignore())            
                .ReverseMap();

        }
    }
}
