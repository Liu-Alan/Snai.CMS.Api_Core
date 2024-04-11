namespace Snai.CMS.Api_Core.Common.Infrastructure
{
    // 返回Code
    public enum Code : int
    {
        Success = 200,
        Fail = 100,
        Error = 500,
        InvalidParams = 400,
        BindParamsError = 401,
        ValidParamsError = 402,
        PermissionFailed = 403,

        RequestError = 10000,
        AuthNotExist = 10001,
        AuthCheckTimeout = 10002,
        AuthCheckFail = 10003,
        AuthFormatFail = 10004,
        RecordNotFound = 20000
    }

    public class Consts
    {
        // 返回消息
        public Dictionary<int, string> MsgInfo = new Dictionary<int, string>
        {
            { (int)Code.Success,"ok"},
            { (int)Code.Fail,"fail"},
            { (int)Code.Error,"Error"},
            { (int)Code.InvalidParams,"参数错误"},
            { (int)Code.BindParamsError,"绑定参数错误"},
            { (int)Code.ValidParamsError,"校验参数错误"},
            { (int)Code.PermissionFailed,"没有权限"},

            { (int)Code.RequestError,"请求错误"},
            { (int)Code.AuthNotExist,"缺少头部参数Authorization"},
            { (int)Code.AuthCheckTimeout,"Authorization超时"},
            { (int)Code.AuthCheckFail,"校验Authorization失败"},
            { (int)Code.AuthFormatFail,"Authorization格式错误"},
            { (int)Code.RecordNotFound,"记录不存在"},
        };

        // 取返回消息
        public string GetMsg(Code code)
        {
            return MsgInfo[(int)code];
        }
    }
}
