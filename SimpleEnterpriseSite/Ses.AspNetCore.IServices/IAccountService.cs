using Ses.AspNetCore.Entities;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework.IService;
using Ses.AspNetCore.Framework.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ses.AspNetCore.IServices
{
    public interface IAccountService
    {
        bool SingIn(string userName, string passWord, out SysUser user, out string message);

        void ChangePwd(string oldPwd, string newPwd);

        void LogOut(string userName);
    }
}
