using System.Dynamic;

namespace Snai.CMS.Api_Core.Common.Infrastructure
{
    public class Message
    {
        ExpandoObject _result;

        #region 构造函数

        /// <summary>
        /// 初始化消息实例
        /// </summary>
        public Message()
        {
            this.Code = 0;
            this.Msg = string.Empty;
            this._result = new ExpandoObject();
        }

        /// <summary>
        /// 初始化消息实例
        /// </summary>
        /// <param name="code">状态代码</param>
        /// <param name="msg">提示信息</param>
        public Message(int code, string msg)
        {
            this.Code = code;
            this.Msg = msg;
            this._result = new ExpandoObject();
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 状态代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public dynamic Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
            }
        }

        #endregion

        #region 公共函数

        /// <summary>
        /// 设置状态代码和提示信息。
        /// </summary>
        /// <param name="code">状态代码</param>
        /// <param name="msg">提示信息</param>
        public void SetMsg(int code, string msg)
        {
            this.Code = code;
            this.Msg = msg;
        }

        #endregion
    }
}
