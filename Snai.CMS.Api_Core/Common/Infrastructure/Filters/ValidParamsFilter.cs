using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Filters
{
    public class ValidParamsFilter: ActionFilterAttribute
    {
        Consts _consts;

        public ValidParamsFilter(Consts consts)
        {
            _consts = consts;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var msg = new Message((int)Code.ValidParamsError, _consts.GetMsg(Code.ValidParamsError));
                ArrayList errorMsg = new ArrayList();
                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        errorMsg.Add(error.ErrorMessage);
                    }
                }
                msg.Result.Data = errorMsg;
                context.Result = new JsonResult(msg);
                return;
            }
            else
            {
                base.OnResultExecuting(context);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
