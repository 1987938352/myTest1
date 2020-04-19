using System.Threading.Tasks;
using Common;
using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
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
       
        public async Task<IActionResult> ChangeRedis(string text)
        {
            string status = await userService.SerRedisAsync(text) ? "ok" : "error";
           // int a =3;
            return Json(new AjaxResult() { Status = status });
        }

        public IActionResult AlertAAA()
        {
            return Json(new AjaxResult() { Status = "ok" });
        }
        public IActionResult SetSession()
        {
            HttpContext.Session.SetString("hu", "rui");
            return Content("rui");
        }
        public IActionResult GetSession()
        {
           string hu=  HttpContext.Session.GetString("hu");
            return Content(hu);
        }
    }
}