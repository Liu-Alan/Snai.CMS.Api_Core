using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Options;
using Snai.CMS.Api_Core.Common.Encrypt;
using Snai.CMS.Api_Core.Common.Infrastructure;
using Snai.CMS.Api_Core.Common.Utils;
using Snai.CMS.Api_Core.Controllers;
using Snai.CMS.Api_Core.DataAccess;
using Snai.CMS.Api_Core.Entities.CMS;
using Snai.CMS.Api_Core.Entities.Settings;
using System.Runtime.CompilerServices;

namespace Snai.CMS.Api_Core.Business
{
    public class CMSBO
    {
        #region 属性声明

        IOptions<LogonSettings> _logonSettings;
        IOptions<PwdSaltSettings> _pwdSaltSettings;
        private readonly ILogger<HomeController> _logger;
        Consts _consts;
        CMSDao _cmsDao;

        #endregion

        #region 构造函数

        public CMSBO(IOptions<LogonSettings> logonSettings, IOptions<PwdSaltSettings> pwdSaltSettings, ILogger<HomeController> logger, Consts consts, CMSDao cmsDao)
        {
            _logonSettings = logonSettings;
            _pwdSaltSettings = pwdSaltSettings;
            _logger = logger;
            _consts = consts;
            _cmsDao = cmsDao;
        }

        #endregion

        #region 账号操作

        //取账号
        public Admin GetAdmin(string userName)
        {
            var userNameE = userName.Trim();
            if (string.IsNullOrEmpty(userNameE))
            {
                return null;
            }
            var admin = _cmsDao.GetAdmin(userNameE);
            if (admin != null && admin.Result.ID > 0)
            {
                return admin.Result;
            }
            else
            {
                return null;
            }
        }

        // 更新账号
        public Message ModifyAdmin(Admin admin)
        {
            var msg = new Message((int)Code.Success, _consts.GetMsg(Code.Success));

            if (admin != null && admin.ID > 0)
            {
                var reAdmin = GetAdmin(admin.UserName);
                if (reAdmin != null && admin.ID != reAdmin.ID)
                {
                    msg.Code = (int)Code.InvalidParams;
                    msg.Msg = "用户名重复";
                    return msg;
                }
                else
                {
                    var state = _cmsDao.ModifyAdmin(admin);
                    if (state.Result)
                    {
                        return msg;
                    }
                    else
                    {
                        msg.Code = (int)Code.InvalidParams;
                        msg.Msg = "用户修改失败";
                        return msg;
                    }
                }
            }
            else
            {
                msg.Code = (int)Code.InvalidParams;
                msg.Msg = "用户不存在";
                return msg;
            }
        }

        #endregion

        #region 用户登录

        //用户登录
        public (Admin, Message) AdminLogin(string userName, string password, string ip)
        {
            var msg = new Message((int)Code.Success, _consts.GetMsg(Code.Success));

            var userNameE = userName.Trim();
            var pwdE = password.Trim();

            if (string.IsNullOrEmpty(userNameE) || string.IsNullOrEmpty(pwdE))
            {
                msg.Code = (int)Code.InvalidParams;
                msg.Msg = "用户名或密码不能为空";
                return (null, msg);
            }

            if (userNameE.Length > 32)
            {
                msg.Code = (int)Code.InvalidParams;
                msg.Msg = "用户名或密码不能为空";
                return (null, msg);
            }

            var admin = GetAdmin(userNameE);
            if (admin == null)
            {
                msg.Code = (int)Code.InvalidParams;
                msg.Msg = "用户名或密码不能为空";
                return (null, msg);
            }

            if (admin.State == 2)
            {
                msg.Code = (int)Code.Error;
                msg.Msg = "用户名或密码不能为空";
                return (null, msg);
            }

            var nowTime = (int)DateTimeUtils.DateTimeToUnixTimeStamp(DateTime.Now);
            if (admin.LockTime > nowTime)
            {
                msg.Code = (int)Code.Error;
                msg.Msg = $"帐号已锁定，请{_logonSettings.Value.LockMinute}分钟后再来登录";
                return (null, msg);
            }

            var role = GetRole(admin.RoleID);
            if (role == null || role.ID <= 0 || role.State == 2)
            {
                msg.Code = (int)Code.Error;
                msg.Msg = "用户角色禁用";
                return (null, msg);
            }

            var pwd = EncryptMd5.EncryptByte(_pwdSaltSettings.Value.Salt + pwdE);
            if (!admin.Password.Equals(pwd, StringComparison.OrdinalIgnoreCase))
            {
                if (admin.ErrorLogonTime + (_logonSettings.Value.ErrorTime * 60) < nowTime)
                {
                    admin.ErrorLogonTime = nowTime;
                    admin.ErrorLogonCount = 1;
                }
                else
                {
                    admin.ErrorLogonCount += 1;
                }

                if (admin.ErrorLogonCount >= _logonSettings.Value.ErrorCount)
                {
                    //锁定账号
                    admin.ErrorLogonTime = 0;
                    admin.ErrorLogonCount = 0;
                    admin.LockTime = nowTime + (_logonSettings.Value.LockMinute * 60);
                    admin.UpdateTime = nowTime;
                    ModifyAdmin(admin);

                    msg.Code = (int)Code.Error;
                    msg.Msg = $"帐号或密码在{_logonSettings.Value.ErrorTime}分钟内，错误{_logonSettings.Value.ErrorCount}次，锁定帐号{_logonSettings.Value.LockMinute}分钟";
                    return (null, msg);
                }
                else
                {
                    //更新错误登录信息
                    ModifyAdmin(admin);

                    msg.Code = (int)Code.Error;
                    msg.Msg = $"帐号或密码错误，如在{_logonSettings.Value.ErrorTime}分钟内，错误{_logonSettings.Value.ErrorCount}次，将锁定帐号{_logonSettings.Value.LockMinute}分钟";
                    return (null, msg);
                }
            }

            //更新账号登录信息
            admin.LastLogonTime = nowTime;
            admin.ErrorLogonTime = 0;
            admin.ErrorLogonCount = 0;
            admin.LockTime = 0;
            admin.LastLogonIP = ip;
            ModifyAdmin(admin);

            return (admin, msg);

        }

        #endregion

        #region 角色管理

        public Role GetRole(int id)
        {
            if (id <= 0)
            {
                return null;
            }
            var role = _cmsDao.GetRole(id);
            if (role != null && role.Result.ID > 0)
            {
                return role.Result;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Token管理

        public Message AddToken(Token token)
        {
            var msg = new Message((int)Code.Success, _consts.GetMsg(Code.Success));

            if (token != null && !string.IsNullOrEmpty(token.TokenStr))
            {
                var state = _cmsDao.AddToken(token);
                if (state.Result)
                {
                    return msg;
                }
                else
                {
                    msg.Code = (int)Code.InvalidParams;
                    msg.Msg = "DB保存Token失败";
                    return msg;
                }
            }
            else
            {
                msg.Code = (int)Code.InvalidParams;
                msg.Msg = "保存Token失败 token 为空";
                return msg;
            }
        }

        public Message ModifyToken(Token token)
        {
            var msg = new Message((int)Code.Success, _consts.GetMsg(Code.Success));

            if (token != null && token.ID>0)
            {
                var state = _cmsDao.ModifyToken(token);
                if (state != null && state.Result)
                {
                    return msg;
                }
                else
                {
                    msg.Code = (int)Code.InvalidParams;
                    msg.Msg = "Tokek修改失败";
                    return msg;
                }
            }
            else
            {
                msg.Code = (int)Code.InvalidParams;
                msg.Msg = "Tokek不存在";
                return msg;
            }
        }

        public Token GetToken(string tokenStr)
        {
            if (string.IsNullOrEmpty(tokenStr))
            {
                return null;
            }
            var token = _cmsDao.GetToken(tokenStr);
            if (token != null && token.Result != null && token.Result.ID > 0)
            {
                return token.Result;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 模块管理

        public Module GetModule(string router)
        {
            if (string.IsNullOrEmpty(router))
            {
                return null;
            }
            var module = _cmsDao.GetModule(router);
            if (module != null && module.Result.ID > 0)
            {
                return module.Result;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 权限管理

        // 取权限
        public RoleModule GetRoleModule(int roleID, int moduleID)
        {
            if (roleID<=0 || moduleID<=0)
            {
                return null;
            }
            var roleModule = _cmsDao.GetRoleModule(roleID, moduleID);
            if (roleModule != null && roleModule.Result.ID > 0)
            {
                return roleModule.Result;
            }
            else
            {
                return null;
            }
        }

        // 判断权限
        public Message VerifyUserRole(string userName,string router)
        {
            var msg = new Message((int)Code.Success, _consts.GetMsg(Code.Success));

            var admin = GetAdmin(userName);
            if (admin == null || admin.ID <= 0)
            {
                msg.Code = (int)Code.RecordNotFound;
                msg.Msg = "账号不存在";
                return msg;
            }

            if (admin.State == 2)
            {
                msg.Code = (int)Code.RecordNotFound;
                msg.Msg = "账号已禁用";
                return msg;
            }

            var role = GetRole(admin.RoleID);
            if (role == null || role.ID <= 0)
            {
                msg.Code = (int)Code.RecordNotFound;
                msg.Msg = "角色不存在";
                return msg;
            }

            if (role.State == 2)
            {
                msg.Code = (int)Code.RecordNotFound;
                msg.Msg = "角色已禁用";
                return msg;
            }

            var module = GetModule(router);
            if (module == null || module.ID <= 0)
            {
                msg.Code = (int)Code.RecordNotFound;
                msg.Msg = "模块不存在";
                return msg;
            }

            if (module.State == 2)
            {
                msg.Code = (int)Code.RecordNotFound;
                msg.Msg = "模块已禁用";
                return msg;
            }

            // 为-1不验证权限
            if (module.ParentID == -1)
            {
                return msg;
            }

            var roleModule = GetRoleModule(role.ID, module.ID);
            if (roleModule == null || roleModule.ID <= 0)
            {
                msg.Code = (int)Code.PermissionFailed;
                msg.Msg = _consts.GetMsg(Code.PermissionFailed);
                return msg;
            }

            return msg;
        }

        #endregion  
    }
}
