using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Filters
{
    public class ExceptionFilter:IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                RedisHelper.GetRedisDatabase().ListLeftPush("ExceptionLog", context.Exception.Message);
                Console.WriteLine($"你没处理列{context.HttpContext.Request.Path}{context.Exception.Message}");
                context.Result = new JsonResult(new { Result = false, Msg = "异常，请联系管理员" });
                context.ExceptionHandled = true;//表示异常已经处理
            }
        }
    }
}
