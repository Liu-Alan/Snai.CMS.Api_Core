using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Filters
{
    public class ValidParamsFilter: ActionFilterAttribute
    {
        Consts _consts;

        public ValidParamsFilter(Consts consts)
        {
            _consts = consts;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var msg = new Message((int)Code.ValidParamsError, _consts.GetMsg(Code.ValidParamsError));

                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        msg.Msg += error.ErrorMessage + "\n";
                    }
                }
                msg.Msg = msg.Msg.TrimEnd('\n');
                context.Result = new JsonResult(msg);
                return;
            }
            else
            {
                context.Result = new OkObjectResult(new Message((int)Code.Success, _consts.GetMsg(Code.Success)));

                base.OnActionExecuting(context);    

            }
        }
    }
}
