using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Ses.AspNetCore.Framework
{
    public class LoginInfoSession
    {
        public string UserId { get; set; }
        public string RealName { get; set; }
        public string DepartmentId { get; set; }
        public string RoleId { get; set; }

        /// <summary>
        /// 通过用户信息获得Claims断言集合
        /// </summary>
        /// <returns></returns>
        public List<Claim> GetClaims()
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim("UserId", UserId ?? string.Empty));
            claims.Add(new Claim("RealName", RealName ?? string.Empty));
            claims.Add(new Claim("DepartmentId", DepartmentId ?? string.Empty));
            claims.Add(new Claim("RoleId", RoleId ?? string.Empty));

            return claims;
        }

        public static LoginInfoSession GetLoginInfoSeesionByPrincipal(IPrincipal principal)
        {
            if (principal is ClaimsPrincipal claims)
            {
                LoginInfoSession loginInfoSession = new LoginInfoSession
                {
                    UserId = claims.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? string.Empty,
                    RealName = claims.Claims.FirstOrDefault(x => x.Type == "RealName")?.Value ?? string.Empty,
                    DepartmentId = claims.Claims.FirstOrDefault(x => x.Type == "DepartmentId")?.Value ?? string.Empty,
                    RoleId = claims.Claims.FirstOrDefault(x => x.Type == "RoleId")?.Value ?? string.Empty,
                };
                return loginInfoSession;
            }
            throw new ArgumentException(message: "The principal must be a ClaimsPrincipal", paramName: nameof(principal));
        }
    }
}
