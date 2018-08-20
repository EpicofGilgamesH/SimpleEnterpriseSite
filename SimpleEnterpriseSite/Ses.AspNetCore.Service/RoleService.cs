using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.Service;
using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Ses.AspNetCore.Framework.IRepository;
using System.Linq;
using Ses.AspNetCore.ViewModels;
using Ses.AspNetCore.Entities.Enum;

namespace Ses.AspNetCore.Services
{
    public class RoleService : BaseService<SysRole, Guid>, IRoleService
    {
        private IRepository<SysUser, Guid> _userRepository;
        private IRepository<UserRole, Guid> _userRoleRepository;
        public RoleService(IRepository<SysRole, Guid> repository
            , IRepository<SysUser, Guid> repository1
            , IRepository<UserRole, Guid> repository2) : base(repository)
        {
            _userRepository = repository1;
            _userRoleRepository = repository2;
        }

        public bool AddRole(SysRole sysRole, List<UserRole> list)
        {
            return _repository.Transcation(x =>
             {
                 x.Add(sysRole);
                 x.AddRange(list);
                 x.SaveChanges();
             });
        }

        public bool UpdateRole(SysRole entity, List<Guid> userList)
        {
            List<UserRole> addUserRoles = new List<UserRole>();
            List<UserRole> deleteUserRoles = new List<UserRole>();
            var urs = new Dictionary<Guid, Guid>();
            foreach (var item in _userRoleRepository.Get(x => x.RoleId == entity.Id))
            {
                urs.Add(item.UserId, item.Id);
            }

            var urList = urs.Keys.ToList();
            var adds = userList.Except(urList); //新增
            var deletes = urList.Except(userList); //删除

            foreach (var item in adds)
            {
                addUserRoles.Add(new UserRole
                {
                    Id = Guid.NewGuid(),
                    RoleId = entity.Id,
                    UserId = item
                });
            }
            foreach (var item in deletes)
            {
                deleteUserRoles.Add(new UserRole
                {
                    Id = urs[item],
                    RoleId = entity.Id,
                    UserId = item
                });
            }

            return _repository.Transcation(x =>
              {
                  x.Update(entity);
                  if (addUserRoles.Count() > 0)
                      x.AddRange(addUserRoles);
                  if (deleteUserRoles.Count() > 0)
                      x.RemoveRange(deleteUserRoles);
                  x.SaveChanges();
              });
        }

        public List<RoleViewModel> GetRolePageList(int pagesize, int pageindex
            , out int totalcount, string role, string user)
        {
            var query = from x in _repository.Entities
                        join a in _userRoleRepository.Entities on x.Id equals a.RoleId into temp1
                        from c in temp1.DefaultIfEmpty()
                        join b in _userRepository.Entities on c.UserId equals b.Id into temp2
                        from d in temp2.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(role) || x.Name.Contains(role))
                        && (string.IsNullOrEmpty(user) || d.UserId.Contains(user) || d.RealName.Contains(user))
                        group x by x into g
                        select g.Key;

            totalcount = query.Count();
            var queryPage = query.GetPageList(pagesize, pageindex).ToList();

            var pageList = from x in queryPage
                           select new RoleViewModel
                           {
                               SysRole = x,
                               //Users = string.Join(",", _userRepository.Get(y => y.RoleId == x.Id).Select(z => $"{z.UserId}({z.RealName})").ToList())
                               Users = string.Join(",", _userRoleRepository.Get(y => y.RoleId == x.Id).Join(_userRepository.Entities, e => e.UserId, o => o.Id, (e, o) =>
                                $"{o.UserId}({o.RealName})"
                               ))
                           };
            return pageList.ToList();
        }

        /// <summary>
        /// 角色删除时，应级联删除对应的user-role数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteRoles(string[] ids)
        {
            var urs = _userRoleRepository.Get(x => ids.Contains(x.RoleId.ToString())).ToList();
            var roles = _repository.Get(x => ids.Contains(x.Id.ToString())).ToList();
            return _repository.Transcation(context =>
             {
                 context.RemoveRange(urs);
                 context.RemoveRange(roles);
                 context.SaveChanges();
             });
        }

        public List<SysUser> GetRoleUserIds(string roleId)
        {
            var result = from x in _userRoleRepository.Entities
                         join y in _userRepository.Entities on x.UserId equals y.Id into temp
                         from z in temp.DefaultIfEmpty()
                         where x.RoleId.ToString() == roleId
                         select new SysUser
                         {
                             Id = z.Id,
                             UserId = z.UserId,
                             RealName = z.RealName
                         };
            return result.ToList();
        }
    }
}
