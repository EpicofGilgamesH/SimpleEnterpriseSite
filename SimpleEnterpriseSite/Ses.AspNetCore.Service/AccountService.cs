using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.IRepository;
using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ses.AspNetCore.Entities.Enum;
using Ses.AspNetCore.Framework.Helper;
using System.Diagnostics;

namespace Ses.AspNetCore.Services
{
    public class AccountService : IAccountService
    {
        private IRepository<SysUser, Guid> _userRepository;
        private IRepository<UserLogin, Guid> _userLoginRepository;

        public AccountService(IRepository<SysUser, Guid> userRepository,
            IRepository<UserLogin, Guid> userLoginRepository)
        {
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
        }
        public void ChangePwd(string oldPwd, string newPwd)
        {
            throw new NotImplementedException();
        }

        public void LogOut(string userName)
        {
            throw new NotImplementedException();
        }

        public bool SingIn(string userName, string passWord, out SysUser user, out string message)
        {
            message = string.Empty;
            user = null;
            //在左连接查询时，需要注意第二个表的值有可能为空***
            //左右连接查询一般在特定的场合用到，而此处的关联连个表查询不需要用到
            //左连接是当右表无关联时，以左表为依据展示所有数据（右表显示默认值）
            //为什么有的时候Linq表达式解析会花费很长的时间？？
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            var userLogin = (from x in _userRepository.Entities
                             join y in _userLoginRepository.Entities
                             on x.Id equals y.SysUserId into temp
                             where x.State == StateEnum.Normal
                             && x.UserId == userName
                             from z in temp
                             select new
                             {
                                 Id = z.Id,
                                 UserId = x.UserId,
                                 RealName = x.RealName,
                                 DepartmentId = x.DepartmentId,
                                 RoleId = x.RoleId,
                                 PassWord = z.UserPassword,
                                 SecretKey = z.UserSecretkey,
                             }).FirstOrDefault();
            //stopwatch.Stop();
            //long aaa = stopwatch.ElapsedMilliseconds;
            if (userLogin == null)
            {
                message = "账户不存在";
                return false;
            }
            // 输入的密码进行 MD5+Encrytp 加密
            var password = PasswordHelper.EncryptMD5Password(passWord, userLogin.SecretKey);
            if (password != userLogin.PassWord)
            {
                message = "用户名密码错误";
                return false;
            }

            // 后续加入数据库日志体系

            var userLoginEntity = _userLoginRepository.Get(x => x.Id == userLogin.Id).FirstOrDefault();
            _userLoginRepository.Edit(userLoginEntity);
            user = new SysUser
            {
                UserId = userLogin.UserId,
                RealName = userLogin.RealName,
                DepartmentId = userLogin.DepartmentId,
                RoleId = userLogin.RoleId,
            };
            return true;
        }
    }
}
