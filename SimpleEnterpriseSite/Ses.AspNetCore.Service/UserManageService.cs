using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.Service;
using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Ses.AspNetCore.Framework.IRepository;
using System.Linq;
using Ses.AspNetCore.Entities.Enum;
using Ses.AspNetCore.Framework.Helper;
using Ses.AspNetCore.ViewModels.UserManage;
using Ses.AspNetCore.Framework.Extesions;

namespace Ses.AspNetCore.Services
{
    public class UserManageService : BaseService<SysUser, Guid>, IUserManageService
    {
        private IRepository<UserLogin, Guid> _loginRepository;
        private IRepository<Department, Guid> _deptRepository;
        private IRepository<UserRole, Guid> _userRoleRepository;

        public UserManageService(IRepository<SysUser, Guid> repository,
           IRepository<UserLogin, Guid> loginRepository,
           IRepository<Department, Guid> deptRepository,
           IRepository<UserRole, Guid> userRoleRepository) : base(repository)
        {
            _loginRepository = loginRepository;
            _deptRepository = deptRepository;
            _userRoleRepository = userRoleRepository;
        }

        public List<UserPageViewModel> GetRolePageList(int pagesize, int pageindex, out int totalcount,
            List<Expression<Func<SysUser, bool>>> searchExpression,
            Dictionary<string, bool> orderDic)
        {
            IQueryable<SysUser> query = _repository.Entities;
            query = query.GetTableQueryable(searchExpression, orderDic, out totalcount);

            var data = from x in query
                       join y in _deptRepository.Entities on x.DepartmentId equals y.Id into temp1
                       from a in temp1.DefaultIfEmpty()
                       select new UserPageViewModel
                       {
                           Id = x.Id.ToString(),
                           UserId = x.UserId,
                           RealName = x.RealName,
                           Gender = x.Gender == null ? "" : x.Gender.ToString(),
                           Birthday = x.Birthday,
                           MobilePhone = x.MobilePhone,
                           Email = x.Email,
                           WeChat = x.WeChat,
                           State = x.State.ToString(),
                           CreationTime = ((DateTime)x.CreationTime).ToString("yyyy-MM-dd HH:mm:ss"),
                           Description = x.Description,
                           DepartmentName = a.Name,
                       };

            return data.GetPageList(pagesize, pageindex).ToList();
        }

        public bool AddUserAndLoinInfo(SysUser user, string userId)
        {
            return _repository.Transcation(context =>
             {
                 user.CreateUserId = userId;
                 user.CreationTime = DateTime.Now;
                 context.Add(user);
                 var userLgoin = InitUserLogin(user);
                 context.Add(userLgoin);
                 context.SaveChanges();
             });
        }

        private UserLogin InitUserLogin(SysUser user)
        {
            UserLogin userLogin = new UserLogin();

            string secretkey = PasswordHelper.NewSecretKey();
            string password = "123456";
            string md5Pwd = PasswordHelper.EncryptPassword(password, secretkey);
            userLogin.SysUserId = user.Id;
            userLogin.UserPassword = md5Pwd;
            userLogin.UserSecretkey = secretkey;

            return userLogin;
        }

        public bool DeleteUser(string[] ids)
        {
            var uls = _loginRepository.Get(x => ids.Contains(x.SysUserId.ToString())).ToList();

            var users = _userRoleRepository.Get(x => ids.Contains(x.UserId.ToString())).ToList();

            var urs = _repository.Get(x => ids.Contains(x.Id.ToString())).ToList();

            return _repository.Transcation(x =>
             {
                 x.RemoveRange(uls);
                 x.RemoveRange(urs);
                 x.RemoveRange(users);
                 x.SaveChanges();
             });

        }

        public List<Guid> GetUIdList(List<string> guids)
        {
            var lists = _repository.Get(x => guids.Contains(x.UserId.ToString())).Select(x => x.Id);
            if (lists.Count() > 0)
                return lists.ToList();
            return null;
        }

        public string GetDeptEncode(string guid)
        {
            try
            {
                Guid deptGuid = new Guid(guid);
                return _deptRepository.Get(x => x.Id == deptGuid).Select(y => y.EnCode).FirstOrDefault();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
