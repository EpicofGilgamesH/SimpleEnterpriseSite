using AutoMapper;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.ViewModels.Icon;
using Ses.AspNetCore.ViewModels.Module;
using Ses.AspNetCore.ViewModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.Framework.AutoMapper
{
    public class SesProfile : Profile, IProfile
    {
        public SesProfile()
        {
            CreateMap<AddUserViewModel, SysUser>()
                .ForMember(x => x.DepartmentId, map => map.MapFrom(vm => vm.DepartmentId));
            CreateMap<ModuleViewModel, SysClaim>();
            CreateMap<IconViewModel, SysIcon>()
               .ForMember(x => x.Icon, map => map.MapFrom(vm => vm.IconNo));
        }
    }
}
