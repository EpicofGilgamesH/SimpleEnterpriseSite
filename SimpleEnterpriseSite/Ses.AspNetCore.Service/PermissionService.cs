using Ses.AspNetCore.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using Ses.AspNetCore.Entities.System;
using Ses.AspNetCore.Framework;
using Ses.AspNetCore.Framework.IRepository;
using System.Linq;
using Ses.AspNetCore.Entities.Enum;
using Ses.AspNetCore.Framework.Model.Permission;
using Ses.AspNetCore.Backstage.Permission;
using Ses.AspNetCore.Framework.Permission;
using Ses.AspNetCore.ViewModels.Permission;
using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Ses.AspNetCore.Framework.Option;

namespace Ses.AspNetCore.Services
{
    public class PermissionService : IPermissionService
    {
        private const string _admin = "Admin";
        private Guid _adminGuid = Guid.Empty;
        private DbContextOption _option;

        private IRepository<SysUser, Guid> _userRepository;
        private IRepository<SysRole, Guid> _roleRepository;
        private IRepository<UserRole, Guid> _userRoleRepository;
        private IRepository<SysClaim, Guid> _claimRepository;
        private IRepository<UserClaim, Guid> _userclaimRepository;
        private IRepository<RoleClaim, Guid> _roleclaimRepository;
        private IRepository<Department, Guid> _deptRepository;
        private IRepository<DepartmentClaim, Guid> _deptclaimRepository;

        public PermissionService(DbContextOption option,
            IRepository<SysUser, Guid> repository1,
            IRepository<SysRole, Guid> repository2,
            IRepository<SysClaim, Guid> repository3,
            IRepository<UserClaim, Guid> repository4,
            IRepository<RoleClaim, Guid> repository5,
            IRepository<Department, Guid> repository6,
            IRepository<DepartmentClaim, Guid> repository7,
            IRepository<UserRole, Guid> repository8)
        {
            _option = option;
            _userRepository = repository1;
            _roleRepository = repository2;
            _claimRepository = repository3;
            _userclaimRepository = repository4;
            _roleclaimRepository = repository5;
            _deptRepository = repository6;
            _deptclaimRepository = repository7;
            _userRoleRepository = repository8;
        }

        #region 私有方法
        /// <summary>
        /// 获得当前用户的所有claims
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        private List<SysClaim> GetAllClaims(LoginInfoSession loginInfo)
        {
            var id = _userRepository.Get(x => x.UserId == loginInfo.UserId).Select(x => x.Id).FirstOrDefault();
            var userClaims = GetUserPermission(id.ToString()).Select(x => x.SysClaim).ToList();
            var roleClaims = GetUserRolePermission(id.ToString()).Select(x => x.SysClaim).ToList();
            var deptClaims = GetDeptPermission(loginInfo.DepartmentId).Select(x => x.SysClaim).ToList();

            userClaims.AddRange(roleClaims);
            userClaims.AddRange(deptClaims);
            return userClaims;
        }
        /// <summary>
        /// 获得当前用户的所有claims
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deptId"></param>
        /// <returns></returns>
        private List<SysClaim> GetAllClaims(string userId, string deptId)
        {
            var id = _userRepository.Get(x => x.UserId == userId.Trim()).Select(x => x.Id).FirstOrDefault();
            var userClaims = GetUserPermission(id.ToString()).Select(x => x.SysClaim).ToList();
            var roleClaims = GetUserRolePermission(id.ToString()).Select(x => x.SysClaim).ToList();
            var deptClaims = GetDeptPermission(deptId.Trim()).Select(x => x.SysClaim).ToList();

            userClaims.AddRange(roleClaims);
            userClaims.AddRange(deptClaims);
            return userClaims;
        }

        /// <summary>
        /// 合并权限集合 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="userPermissions"></param>
        private void UnionPermission(List<CompletePemission> tagetList, List<CompletePemission> currentList)
        {
            if (currentList != null)
            {
                foreach (var item in currentList)
                {
                    if (tagetList.Exists(x => x.SysClaim.Id == item.SysClaim.Id))
                    {
                        var current = tagetList.Find(x => x.SysClaim.Id == item.SysClaim.Id);
                        current.BtnPermission += item.BtnPermission;
                    }
                    else
                    {
                        tagetList.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 获取查询条件 
        /// </summary>
        /// <param name="btnPermission"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        private string GetCondition(string btnPermission, string userName)
        {
            var condition = string.Empty;
            if (!string.IsNullOrEmpty(btnPermission))
            {
                var btns = btnPermission.Split(',');
                for (int i = 0; i < btns.Length; i++)
                {
                    if (!string.IsNullOrEmpty(btns[i]))
                    {
                        if (i == 0)
                            condition += string.Format(" WHERE c.btn LIKE '%{0}%' ", btns[i]);
                        else
                            condition += string.Format("AND c.btn LIKE '%{0}%' ", btns[i]);
                    }
                }
            }
            if (!string.IsNullOrEmpty(userName))
            {
                var cond = string.Format(" (c.RealName LIKE '%{0}%' or c.UserId LIKE '%{0}%') ", userName.Trim());
                if (!string.IsNullOrEmpty(condition))
                {
                    condition += " AND " + cond;
                }
                else
                {
                    condition += " WHERE " + cond;
                }
            }

            return condition;
        }

        /// <summary>
        /// 获取mssql的分页sql和总条数sql
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private Tuple<string, string> GetMSSql(string condition, int pageindex, int pageszie)
        {
            #region 分页sql
            var sqlpage = string.Format(@"WITH result as (
                                 SELECT[t2].[UserId], [t2].[RealName], [t0].[BtnPermission]
                                    FROM[RoleClaim] AS[t0]
                                        INNER JOIN[UserRole] AS[t1] ON[t0].[RoleId] = [t1].[RoleId]
                                    INNER JOIN[SysUser] AS[t2] ON[t1].[UserId] = [t2].[Id]
                                    WHERE[t0].[ClaimId] =@claimid

                                        UNION ALL
                                 SELECT[t5].[UserId], [t5].[RealName], [t4].[BtnPermission]
                                    FROM[UserClaim] AS[t4]
                                        INNER JOIN[SysUser] AS[t5] ON[t4].[UserId] = [t5].[Id]
                                    WHERE[t4].[ClaimId] = @claimid

                                        UNION ALL
                                 SELECT[t9].[UserId], [t9].[RealName], [t7].[BtnPermission]
                                    FROM[DepartmentClaim] AS[t7]
                                        INNER JOIN[Department] AS[t8] ON[t7].[DepartmentId] = [t8].[Id]
                                    INNER JOIN[SysUser] AS[t9] ON([t8].[Id]) = [t9].[DepartmentId]
                                    WHERE[t7].[ClaimId] = @claimid
			                            ) 
			                            SELECT TEMP.UserId,TEMP.RealName FROM
                                        (
                                        SELECT*, ROW_NUMBER() OVER(ORDER BY userid) AS ind FROM
                                        (
                                        SELECT * FROM
                                        (
                                        SELECT[UserId],[RealName],
                                        STUFF((SELECT ' ' + BtnPermission FROM result a WHERE b.UserId = a.UserId FOR XML PATH('')), 1, 1, '') btn
                                         FROM result b

                                        GROUP BY b.UserId, b.RealName
                                        ) c
                                        {0}
			                            ) d
			                            ) AS TEMP
                                        WHERE TEMP.ind BETWEEN({1} and {2}", condition, pageindex * pageszie + 1, pageszie * (pageindex + 1));

            #endregion

            #region 总条数sql
            var sqlcount = string.Format(@"WITH result as (
	                                             SELECT [t2].[UserId], [t2].[RealName], [t0].[BtnPermission]
                                                        FROM [RoleClaim] AS [t0]
                                                        INNER JOIN [UserRole] AS [t1] ON [t0].[RoleId] = [t1].[RoleId]
                                                        INNER JOIN [SysUser] AS [t2] ON [t1].[UserId] = [t2].[Id]
                                                        WHERE [t0].[ClaimId]= @claimid

			                                            UNION ALL  
	                                             SELECT [t5].[UserId], [t5].[RealName], [t4].[BtnPermission]
                                                        FROM [UserClaim] AS [t4]
                                                        INNER JOIN [SysUser] AS [t5] ON [t4].[UserId] = [t5].[Id]
                                                        WHERE [t4].[ClaimId] = @claimid

			                                            UNION ALL
                                                 SELECT [t9].[UserId], [t9].[RealName], [t7].[BtnPermission]
			                                            FROM [DepartmentClaim] AS [t7]
			                                            INNER JOIN [Department] AS [t8] ON [t7].[DepartmentId] = [t8].[Id]
			                                            INNER JOIN [SysUser] AS [t9] ON ([t8].[Id]) = [t9].[DepartmentId]
			                                            WHERE [t7].[ClaimId] = @claimid
			                                            ) 

					                                            SELECT count(1) FROM
			                                            (
			                                            SELECT [UserId],[RealName],
			                                            STUFF((SELECT ' '+ BtnPermission FROM result a WHERE b.UserId =a.UserId FOR XML PATH('')),1,1,'') btn
			                                            FROM result b
			                                            GROUP BY b.UserId,b.RealName
			                                            ) c
			                                            {0} ", condition);
            #endregion

            return Tuple.Create(sqlpage, sqlcount);
        }

        /// <summary>
        /// 获取mysql的分页sql和总条数sql
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageindex"></param>
        /// <param name="pageszie"></param>
        /// <returns></returns>
        private Tuple<string, string> GetMYSql(string condition, int pageindex, int pageszie)
        {
            #region mysql分页
            var mysqlpage = string.Format(@" select UserId,RealName,GROUP_CONCAT(BtnPermission SEPARATOR ',') BtnPermission from result {0}
                                      group by UserId,RealName limit {1},{2};
                                      drop temporary TABLE IF EXISTS result;", condition, pageindex * pageszie + 1, pageszie);
            #endregion

            #region 总条数

            var mysqltotal = @" CREATE TEMPORARY TABLE result
                                    SELECT t2.UserId AS UserId, t2.RealName AS RealName, t0.BtnPermission AS BtnPermission
                                    FROM RoleClaim t0

                                    INNER JOIN UserRole t1 ON t0.RoleId = t1.RoleId
                                    INNER JOIN SysUser t2 ON t1.UserId = t2.Id
                                    WHERE t0.ClaimId = @claimid

                                    UNION ALL
                                    SELECT t5.UserId AS UserId, t5.RealName AS RealName, t4.BtnPermission AS BtnPermission
                                    FROM UserClaim t4

                                    INNER JOIN SysUser t5 ON t4.UserId = t5.Id
                                    WHERE t4.ClaimId =  @claimid

                                    UNION ALL
                                    SELECT t9.UserId AS UserId, t9.RealName AS RealName, t7.BtnPermission AS BtnPermission
                                    FROM DepartmentClaim t7

                                    INNER JOIN Department t8 ON t7.DepartmentId = t8.Id
                                    INNER JOIN SysUser t9 ON(t8.Id) = t9.DepartmentId
                                    WHERE t7.ClaimId = @claimid; 
                                    select count(1) from result; ";
            #endregion
            return Tuple.Create(mysqlpage, mysqltotal);
        }


        #endregion

        /// <summary>
        /// 获取人员页面权限并集(用户登陆时加载页面时的判断)
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        public List<SysClaim> GetUnionPermission(LoginInfoSession loginInfo)
        {
            // 管理员拥有所有权限
            if (loginInfo.UserId.ToUpper() == _admin.ToUpper())
            {
                return GetAllPermission();
            }
            else
            {
                List<SysClaim> userClaims = GetAllClaims(loginInfo);

                return userClaims.Distinct(new ClaimCompare()).OrderBy(x => x.Sort).ToList();
            }
        }

        /// <summary>
        /// 获取人员页面权限并集
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="deptId">部门Id</param>
        /// <returns></returns>
        public List<CompletePemission> GetUnionPermission(string userId)
        {
            var user = _userRepository.Get(x => x.Id.ToString() == userId).FirstOrDefault();
            var deptId = user.DepartmentId;
            List<CompletePemission> list = new List<CompletePemission>();
            var userPermissions = GetUserPermission(userId);

            var rolePermissions = GetUserRolePermission(userId);

            List<CompletePemission> deptPermissions = null;
            if (deptId != null)
            {
                deptPermissions = GetDeptPermission(deptId.ToString());
            }

            UnionPermission(list, userPermissions);

            UnionPermission(list, rolePermissions);

            UnionPermission(list, deptPermissions);

            return list;
        }


        /// <summary>
        /// 获取用户按钮权限并集 
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        public string GetUnionBtnPermission(LoginInfoSession loginInfo, string url)
        {
            if (loginInfo.UserId.ToUpper() == _admin.ToUpper())
            {
                return BtnPermission.AllBtnPms;
            }
            else
            {
                var claim = _claimRepository.Entities.Where(x => x.Url.ToUpper() == url.ToUpper()).FirstOrDefault();
                if (claim != null)
                {
                    //用户
                    var uid = (from x in _userRepository.Entities
                               where x.UserId == loginInfo.UserId && x.State == StateEnum.Normal
                               select x.Id).FirstOrDefault();
                    string userBtn = string.Empty;
                    string deptBtn = string.Empty;
                    string roleBtn = string.Empty;

                    var userEntity = _userclaimRepository.Get(x => x.ClaimId == claim.Id
                    && x.UserId == uid).FirstOrDefault();
                    if (userEntity != null)
                        userBtn = userEntity.BtnPermission;
                    //部门
                    var deptEntity = _deptclaimRepository.Get(x => x.ClaimId == claim.Id
                    && x.DepartmentId.ToString() == loginInfo.DepartmentId).FirstOrDefault();
                    if (deptEntity != null)
                        deptBtn = deptEntity.BtnPermission;
                    //角色
                    //var roleEntity = _roleclaimRepository.Get(x => x.ClaimId == claim.Id
                    //  && x.RoleId.ToString() == loginInfo.RoleId).FirstOrDefault();
                    var roleEntity = (from x in _userRoleRepository.Entities
                                      join y in _roleclaimRepository.Entities on x.RoleId equals y.RoleId into temp1
                                      from a in temp1.DefaultIfEmpty()
                                      where x.UserId == uid && a.ClaimId == claim.Id
                                      select a).FirstOrDefault();

                    if (roleEntity != null)
                        roleBtn = roleEntity.BtnPermission;
                    return userBtn + deptBtn + roleBtn;
                }
                return string.Empty;
            }
        }


        /// <summary>
        /// 获取人员权限
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<CompletePemission> GetUserPermission(string userId)
        {
            Guid userGuid = new Guid(userId);
            if (userGuid == _adminGuid)
            {
                List<CompletePemission> list = new List<CompletePemission>();
                var claims = GetAllPermission();
                return claims.Select(x => new CompletePemission
                { SysClaim = x, BtnPermission = BtnPermission.AllBtnPms }).ToList();
            }
            //var models = from x in _userRepository.Entities.
            //             Where(x => x.Id == userGid && x.State == StateEnum.Normal)
            //             join y in _userclaimRepository.Entities on x.Id equals y.UserId
            //             join z in _claimRepository.Entities on y.ClaimId equals z.Id into temp
            //             from i in temp.DefaultIfEmpty()
            //             select new CompletePemission { SysClaim = i, BtnPermission = y.BtnPermission };

            var models =
                         from y in _userclaimRepository.Entities
                         join z in _claimRepository.Entities on y.ClaimId equals z.Id into temp
                         where y.UserId == userGuid
                         from i in temp.DefaultIfEmpty()
                         select new CompletePemission { SysClaim = i, BtnPermission = y.BtnPermission };
            return models.ToList();
        }

        /// <summary>
        /// 获取该人员的所有角色权限
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public List<CompletePemission> GetUserRolePermission(string userId)
        {
            try
            {
                Guid userGuid = new Guid(userId);

                //var models = from x in _roleRepository.Get(x => x.Id == guidRoleId)
                //             join y in _roleclaimRepository.Entities on x.Id equals y.RoleId
                //             join z in _claimRepository.Entities on y.ClaimId equals z.Id into temp
                //             from i in temp.DefaultIfEmpty()
                //             select new CompletePemission { SysClaim = i, BtnPermission = y.BtnPermission };

                var models = from x in _roleclaimRepository.Entities
                             join y in _userRoleRepository.Entities on x.RoleId equals y.RoleId
                             join z in _claimRepository.Entities on x.ClaimId equals z.Id into temp2
                             from i in temp2.DefaultIfEmpty()
                             where y.UserId == userGuid
                             select new CompletePemission { SysClaim = i, BtnPermission = x.BtnPermission };
                return models.ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取部门权限
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public List<CompletePemission> GetDeptPermission(string deptId)
        {
            try
            {
                Guid guidDepd = new Guid(deptId);
                //var models = from x in _deptRepository.Get(x => x.Id == guidDepd)
                //             join y in _deptclaimRepository.Entities on x.Id equals y.DepartmentId
                //             join z in _claimRepository.Entities on y.ClaimId equals z.Id into temp
                //             from i in temp.DefaultIfEmpty()
                //             select new CompletePemission { SysClaim = i, BtnPermission = y.BtnPermission };

                var models = from y in _deptclaimRepository.Entities
                             join z in _claimRepository.Entities on y.ClaimId equals z.Id into temp
                             where y.DepartmentId == guidDepd
                             from i in temp.DefaultIfEmpty()
                             select new CompletePemission { SysClaim = i, BtnPermission = y.BtnPermission };
                return models.ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获得管理员所有权限
        /// </summary>
        /// <returns></returns>
        public List<SysClaim> GetAllPermission()
        {
            return _claimRepository.Entities.Where(x => x.State == StateEnum.Normal).OrderBy(x => x.Sort).ToList();
        }

        public bool OperateUserPermission(string userId, List<PermissionData> permissionData)
        {
            //没有选中或取消选中的module该如何处理
            List<UserClaim> edits = new List<UserClaim>();
            List<UserClaim> adds = new List<UserClaim>();
            foreach (var pd in permissionData)
            {
                var uc = _userclaimRepository.EntitiesNoTracking.Where
                             (x => x.UserId.ToString() == userId && x.ClaimId == pd.peId).FirstOrDefault();
                if (uc != null)
                {
                    if (uc.BtnPermission != pd.btn)
                    {
                        uc.BtnPermission = pd.btn;
                        edits.Add(uc);
                    }
                }
                else
                {
                    UserClaim userClaim = new UserClaim()
                    {
                        UserId = new Guid(userId),
                        ClaimId = pd.peId,
                        BtnPermission = pd.btn
                    };
                    adds.Add(userClaim);
                }
            }

            var all = _userclaimRepository.EntitiesNoTracking.Where(x => x.UserId.ToString() == userId).ToList();
            foreach (var item in all)
            {
                //在Foreach循环中 同一个context实例还在做查询动作，
                //此时这个context实例再做其它动作，会出现线程占用报错，
                //因为Linq的延时执行特性
                if (!permissionData.Exists(x => x.peId == item.ClaimId))
                    _userclaimRepository.Delete(item);
            }
            var flag1 = _userclaimRepository.AddRange(adds);
            var flag2 = _userclaimRepository.EditRange(edits);

            return flag1 != -1 && flag2 != -1;
        }

        public bool OperateRolePermission(string roleId, List<PermissionData> permissionData)
        {
            List<RoleClaim> edits = new List<RoleClaim>();
            List<RoleClaim> adds = new List<RoleClaim>();
            foreach (var pd in permissionData)
            {
                var rc = _roleclaimRepository.EntitiesNoTracking.Where
                             (x => x.RoleId.ToString() == roleId && x.ClaimId == pd.peId).FirstOrDefault();
                if (rc != null)
                {
                    if (rc.BtnPermission != pd.btn)
                    {
                        rc.BtnPermission = pd.btn;
                        edits.Add(rc);
                    }
                }
                else
                {
                    RoleClaim userClaim = new RoleClaim()
                    {
                        RoleId = new Guid(roleId),
                        ClaimId = pd.peId,
                        BtnPermission = pd.btn
                    };
                    adds.Add(userClaim);
                }
            }
            var all = _roleclaimRepository.EntitiesNoTracking.Where(x => x.RoleId.ToString() == roleId).ToList();
            foreach (var item in all)
            {
                if (!permissionData.Exists(x => x.peId == item.ClaimId))
                    _roleclaimRepository.Delete(x => x.RoleId.ToString() == roleId
                    && x.ClaimId == item.ClaimId);
            }
            var flag1 = _roleclaimRepository.AddRange(adds);
            var flag2 = _roleclaimRepository.EditRange(edits);
            return flag1 != -1 && flag2 != -1;
        }

        public bool OperateDeptPermission(string deptId, List<PermissionData> permissionData)
        {
            List<DepartmentClaim> edits = new List<DepartmentClaim>();
            List<DepartmentClaim> adds = new List<DepartmentClaim>();
            foreach (var pd in permissionData)
            {
                var dc = _deptclaimRepository.Get
                             (x => x.DepartmentId.ToString() == deptId && x.ClaimId == pd.peId).FirstOrDefault();
                if (dc != null)
                {
                    if (dc.BtnPermission != pd.btn)
                    {
                        dc.BtnPermission = pd.btn;
                        edits.Add(dc);
                    }
                }
                else
                {
                    DepartmentClaim userClaim = new DepartmentClaim()
                    {
                        DepartmentId = new Guid(deptId),
                        ClaimId = pd.peId,
                        BtnPermission = pd.btn
                    };
                    adds.Add(userClaim);
                }
            }
            var flag1 = _deptclaimRepository.AddRange(adds);
            var flag2 = _deptclaimRepository.EditRange(edits);
            var all = _deptclaimRepository.Entities.Where(x => x.DepartmentId.ToString() == deptId).ToList();
            //删除该部门权限中取消了的权限
            foreach (var item in all)
            {
                if (!permissionData.Exists(x => x.peId == item.ClaimId))
                    _deptclaimRepository.Delete(x => x.DepartmentId.ToString() == deptId
                    && x.ClaimId == item.ClaimId);
            }
            return flag1 != -1 && flag2 != -1;
        }

        /// <summary>
        /// 通过角色Id获取其权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<CompletePemission> GetRolePermission(string roleId)
        {
            try
            {
                Guid roleGuid = new Guid(roleId);
                //var models = from x in _roleRepository.Get(x => x.Id == guidRoleId)
                //             join y in _roleclaimRepository.Entities on x.Id equals y.RoleId
                //             join z in _claimRepository.Entities on y.ClaimId equals z.Id into temp
                //             from i in temp.DefaultIfEmpty()
                //             select new CompletePemission { SysClaim = i, BtnPermission = y.BtnPermission };

                var models = from x in _roleclaimRepository.Entities
                             join z in _claimRepository.Entities on x.ClaimId equals z.Id into temp2
                             from i in temp2.DefaultIfEmpty()
                             where x.RoleId == roleGuid
                             select new CompletePemission { SysClaim = i, BtnPermission = x.BtnPermission };
                return models.ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 页面权限对应的
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns></returns>
        //    public List<ClaimUserModel> GetRolePermissionUsers(string claimId, string btnPermission
        //        , out int totalCount, int pageindex = 0, int pageSize = 10)
        //    {
        //        //权限对于的角色用户   内连接，当该角色没有用户时，没有记录
        //        var useList = (from x in _roleclaimRepository.Entities
        //                       join y in _userRoleRepository.Entities on x.RoleId equals y.RoleId
        //                       join z in _userRepository.Entities on y.UserId equals z.Id
        //                       where x.ClaimId.ToString() == claimId && z.State == StateEnum.Normal
        //                       group new { z.UserId, z.RealName, x.BtnPermission }
        //                       by new { z.UserId, z.RealName, x.BtnPermission } into c
        //                       select new
        //                       {
        //                           UserId = c.Key.UserId,
        //                           UserName = c.Key.RealName,
        //                           btnPermissions = c.Key.BtnPermission
        //                       }).Union(
        //            from x in _userclaimRepository.Entities
        //            join y in _userRepository.Entities on x.UserId equals y.Id
        //            where x.ClaimId.ToString() == claimId && y.State == StateEnum.Normal
        //            select new
        //            {
        //                UserId = y.UserId,
        //                UserName = y.RealName,
        //                btnPermissions = x.BtnPermission
        //            }).Union(
        //            from x in _deptclaimRepository.Entities
        //            join y in _deptRepository.Entities on x.DepartmentId equals y.Id
        //            join z in _userRepository.Entities on y.Id equals z.DepartmentId
        //            where x.ClaimId.ToString() == claimId && z.State == StateEnum.Normal
        //            select new
        //            {
        //                UserId = z.UserId,
        //                UserName = z.RealName,
        //                btnPermissions = x.BtnPermission
        //            });

        //        //var list = from x in useList
        //        //           group x by new { x.UserId, x.UserName } into y
        //        //           let btns = y.Select(z => z.btnPermissions).ToArray().Distinct()
        //        //           select new { y.Key.UserId, y.Key.UserName, btn = String.Join("", String.Join("", btns).Split(',').Distinct().OrderBy(a => a)) };

        //        var bbb = useList.ToList();

        //        var queryable = useList.GroupBy(x => x.UserId).Select(y => y.Key);


        //        //if (!string.IsNullOrEmpty(btnPermission))
        //        //{
        //        //    foreach (var item in btnPermission.Split(','))
        //        //    {
        //        //        queryable = queryable.Where(x => x.btn.Contains(item));
        //        //    }
        //        //}

        //        var aaaa = queryable.ToList();

        //        totalCount = queryable.Count();

        //        return null;
        //        //return queryable.Select(x => new ClaimUserModel { UserId = x.UserId, UserName = x.UserName }).Skip(pageindex).Take(pageSize).ToList();
        //    }
        //}

        /// <summary>
        /// 页面权限对应的
        /// </summary>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public List<ClaimUserModel> GetRolePermissionUsers(string claimId, string btnPermission,
            string userName, out int totalCount, int pageindex = 0, int pagesize = 10)
        {
            if (string.IsNullOrEmpty(claimId))
            {
                totalCount = 0;
                return null;
            }
            //权限对于的角色用户   内连接，当该角色没有用户时，没有记录
            //sql条件
            string condition = GetCondition(btnPermission, userName);

            Tuple<string, string> tuple = null;
            List<ClaimUserModel> claimUserList = null;
            int total = 0;
            if (_option.DbType == DbTypeEnum.MSSQLSERVER)
            {
                var param = new SqlParameter[] { new SqlParameter("@claimid", claimId) };
                tuple = GetMSSql(condition, pageindex, pagesize);
                total = _userRepository.RawSqlQuery(tuple.Item2, x => (int)x[0], param).FirstOrDefault();
                //同一个Parameters 会进行引用地址检查，不能将相同的引用地址的Parameters用到多个sql语句中
                var newParameters = param.Select(x => (SqlParameter)((ICloneable)x).Clone()).ToArray();
                claimUserList = _userRepository.RawSqlQuery(tuple.Item1, x => new ClaimUserModel { UserId = (string)x[0], RealName = (string)x[1] }, newParameters).ToList();
            }
            else if (_option.DbType == DbTypeEnum.MYSQL)
            {
                var mysqlparam = new MySqlParameter[] { new MySqlParameter("@claimid", claimId) };
                tuple = GetMYSql(condition, pageindex, pagesize);
                total = Convert.ToInt32(_userRepository.RawSqlQuery(tuple.Item2, x => (Int64)x[0], mysqlparam).FirstOrDefault());
                var newmysqlparam = new MySqlParameter[] { new MySqlParameter("@claimid", claimId) };
                claimUserList = _userRepository.RawSqlQuery(tuple.Item1, x => new ClaimUserModel { UserId = (string)x[0], RealName = (string)x[1] }, newmysqlparam).ToList();
            }
            totalCount = total;
            return claimUserList;
        }


    }

    public class ClaimCompare : IEqualityComparer<SysClaim>
    {
        public bool Equals(SysClaim x, SysClaim y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(SysClaim obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
