using Microsoft.EntityFrameworkCore;
using Snai.CMS.Api_Core.Entities.CMS;
using System;

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
            var upAdmin = _cmsContext.Admins.SingleOrDefault(s => s.ID == admin.ID);
            if (upAdmin != null && admin.ID > 0)
            {
                upAdmin.UserName = admin.UserName;
                upAdmin.Password = admin.Password;
                upAdmin.RoleID = admin.RoleID;
                upAdmin.State = admin.State;
                upAdmin.CreateTime = admin.CreateTime;
                upAdmin.UpdateTime = admin.UpdateTime;
                upAdmin.LastLogonTime = admin.UpdateTime;
                upAdmin.LastLogonIP = admin.LastLogonIP;
                upAdmin.ErrorLogonTime = admin.ErrorLogonTime;
                upAdmin.ErrorLogonCount = admin.ErrorLogonCount;
                upAdmin.LockTime = admin.LockTime;

                return await _cmsContext.SaveChangesAsync() > 0;
            }

            return upState;
        }

        #endregion

        #region 角色管理

        // 取角色
        public async Task<Role> GetRole(int id)
        {
            return await _cmsContext.Roles.FirstOrDefaultAsync(s => s.ID == id);
        }
        #endregion
    }
}
