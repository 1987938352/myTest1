using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using IService;
using Microsoft.AspNetCore.Mvc;

namespace ZP.Controllers
{
    public class TestController : Controller
    {
        private readonly IUserService userService;

        public TestController(IUserService userService)
        {
            this.userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRedis(string Text)
        {
            string status = await userService.SerRedisAsync(Text) ? "ok" : "error";
            return Json(new AjaxResult() {Status= status });
        } 
    }
}