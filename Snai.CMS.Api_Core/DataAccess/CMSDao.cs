using Microsoft.EntityFrameworkCore;
using Snai.CMS.Api_Core.Entities.CMS;
using System;
using System.Linq;

namespace Snai.CMS.Api_Core.DataAccess
{
    public class CMSDao
    {
        #region 属性声明

        CMSContext _cmsContext;

        #endregion

        #region 构造函数

        public CMSDao(CMSContext context)
        {
            _cmsContext = context;
        }

        #endregion

        #region 账号管理

        //取账号
        public async Task<Admin> GetAdmin(string userName)
        {
            return await _cmsContext.Admins.FirstOrDefaultAsync(s => s.UserName == userName);
        }

        //修改账号
        public async Task<bool> ModifyAdmin(Admin admin)
        {
            var upState = false;
            var upAdmin = _cmsContext.Admins.FirstOrDefault(s => s.ID == admin.ID);
            if (upAdmin != null && admin.ID > 0)
            {
                upAdmin.UserName = admin.UserName;
                upAdmin.Password = admin.Password;
                upAdmin.RoleID = admin.RoleID;
                upAdmin.State = admin.State;
                upAdmin.CreateTime = admin.CreateTime;
                upAdmin.UpdateTime = admin.UpdateTime;
                upAdmin.LastLogonTime = admin.LastLogonTime;
                upAdmin.LastLogonIP = admin.LastLogonIP;
                upAdmin.ErrorLogonTime = admin.ErrorLogonTime;
                upAdmin.ErrorLogonCount = admin.ErrorLogonCount;
                upAdmin.LockTime = admin.LockTime;

                return await _cmsContext.SaveChangesAsync() > 0;
            }

            return upState;
        }

        //取用户总数
        public long GetAdminCount(string userName)
        {
            var admins =  _cmsContext.Admins.ToList();
            if (!string.IsNullOrEmpty(userName))
            {
                admins = admins.Where(d => d.UserName.Contains(userName)).ToList();
            }
            return admins.Count; 
        }

        //取账号列表
        public List<Admin> GetAdminList(string userName, int pageOffset, int pageSize)
        {
            var admins = _cmsContext.Admins.ToList();
            if (!string.IsNullOrEmpty(userName))
            {
                admins = admins.Where(d => d.UserName.Contains(userName)).ToList();
            }

            if (pageOffset >= 0 && pageSize > 0)
            {
                admins = admins.Skip(pageOffset).Take(pageSize).ToList();
            }

            return admins;  
        }

        #endregion

        #region 角色管理

        // 取角色
        public async Task<Role> GetRole(int id)
        {
            return await _cmsContext.Roles.FirstOrDefaultAsync(s => s.ID == id);
        }

        #endregion

        #region Token管理

        // 添加Token
        public async Task<bool> AddToken(Token token)
        {
            await _cmsContext.Tokens.AddAsync(token);
            return await _cmsContext.SaveChangesAsync() > 0;
        }

        //修改Token
        public async Task<bool> ModifyToken(Token token)
        {
            var upState = false;
            var upToken = _cmsContext.Tokens.FirstOrDefault(s => s.ID == token.ID);
            if (upToken != null && upToken.ID > 0)
            {
                upToken.TokenStr = token.TokenStr;
                upToken.UserID = token.UserID;
                upToken.State = token.State;
                upToken.CreateTime = token.CreateTime;

                return await _cmsContext.SaveChangesAsync() > 0;
            }

            return upState;
        }

        //取Token
        public async Task<Token> GetToken(string tokenStr)
        {
            return await _cmsContext.Tokens.FirstOrDefaultAsync(s => s.TokenStr == tokenStr);
        }

        #endregion

        #region 模块管理

        //取模块
        public async Task<Module> GetModule(string router)
        {
            return await _cmsContext.Modules.FirstOrDefaultAsync(s => s.Router == router);
        }

        #endregion

        #region 权限管理

        //取模块
        public async Task<RoleModule> GetRoleModule(int roleID, int moduleID)
        {
            return await _cmsContext.RoleModules.FirstOrDefaultAsync(s => s.RoleID == roleID && s.ModuleID == moduleID);
        }

        #endregion
    }
}
