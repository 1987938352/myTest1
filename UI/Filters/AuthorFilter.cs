using Common;
using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UI.Attributes;

namespace UI.Filters
{
    public class AuthorFilter : IAuthorizationFilter
    {
        private readonly IUserService userService;

        public AuthorFilter(IUserService userService)
        {
            this.userService = userService;
        }
        public  void OnAuthorization(AuthorizationFilterContext context)
        {
            CheckPermissionAttribute[] permAtts = (CheckPermissionAttribute[])((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttributes(typeof(CheckPermissionAttribute), false);//返回当前要指向的Action上标注CheckPermissionAttribute的特性标签数组
            if (permAtts.Length<=0)
            {
                return;
            }
            string StrUserId = context.HttpContext.Session.GetString("UserId");
            long UserId;
            if (!Int64.TryParse(StrUserId,out UserId))
            {
                if (AjaxRequestHelper.isAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new JsonResult(new AjaxResult() { Status = "redirect", Data = "/Main/Login", ErrorMsg = "没有登录" });
                    return;
                }
                else
                {
                    context.Result = new RedirectResult("/Main/Login");
                    return;
                }
            }
            if (permAtts.Count()==1&&string.IsNullOrWhiteSpace( permAtts[0].Role))
            {
                return;
            }
            foreach (CheckPermissionAttribute attr in permAtts)
            {
                if (attr.Role!="Login")
                {
                    if ( !userService.HasRole(UserId, attr.Role))
                    {
                        if (AjaxRequestHelper.isAjaxRequest(context.HttpContext.Request))
                        {
                            AjaxResult ajaxResult = new AjaxResult() { Status = "error", ErrorMsg = "没有权限" + UserId };
                            context.Result = new JsonResult(ajaxResult);
                        }
                        else
                        {
                            context.Result = new ContentResult() { Content = "没有权限" + attr.Role };
                        }
                    }
                }
            }
            return;
        }
    }
}
