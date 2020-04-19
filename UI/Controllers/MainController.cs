using CaptchaGen.NetCore;
using Common;
using IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ModelDTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Service;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UI.Attributes;
using UI.Models;

namespace UI.Controllers
{
    public class MainController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IUserService userService;
        private readonly IConfiguration configuration;

        public MainController(IWebHostEnvironment webHostEnvironment,IUserService userService,IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.userService = userService;
            this.configuration = configuration;
        }
        [CheckPermission("")]
        public async Task< IActionResult> Index(int PageIndex=1)
        {
            List<long> Id_List = new List<long>();
            ViewBag.pageIndex = 5;
            ViewBag.totalCount = await RedisHelper.GetRedisDatabase().SortedSetLengthAsync("PicScore");
            if (!await RedisHelper.GetRedisDatabase().KeyExistsAsync($"SortPic{PageIndex}"))
            {
                int pageSize = 5;
                var datas = await RedisHelper.GetRedisDatabase().SortedSetRangeByRankWithScoresAsync("PicScore", (PageIndex - 1) * pageSize, PageIndex * pageSize);
                Id_List = datas.Select(d => Convert.ToInt64(d.Element)).ToList();
                string SerStr = JsonConvert.SerializeObject(Id_List);
              await  RedisHelper.GetRedisDatabase().StringSetAsync($"SortPic{PageIndex}", SerStr);
            }
            else
            {
                string SerStr =await RedisHelper.GetRedisDatabase().StringGetAsync($"SortPic{PageIndex}");
                 Id_List = (List<long>)JsonConvert.DeserializeObject(SerStr);
            }

            return View(Id_List);
        }
        
        [CheckPermission("admin")]
        public async Task<IActionResult> Admin(int pageIndex = 1)
        {
            int pageSize = 10;
            UserDTO[] users =await userService.GetAllUserAsync((pageIndex-1)*pageSize,pageSize);
            long totalCount = await userService.GetAllCountAsync();
            ViewBag.pageIndex = pageIndex;
            ViewBag.totalCount = totalCount;
            return View(users);
        }
        public async Task<IActionResult> DeleteById(long Id)
        {
            bool isTrue = await userService.DeleteByIdAsync(Id);
            AjaxResult result = new AjaxResult();
            if (isTrue)
            {
                result.Status = "ok"; 
            }
            else
            {
                result.Status = "error";
                result.ErrorMsg = "删除错误";
            }
            return Json(result);
        }
        public async Task<IActionResult> LoginOrRegister()
        {
            return await Task.Run(() => { return View(); });
        }
        public async Task<IActionResult> Login()
        {
            return await Task.Run(() => { return View(); });
        }
        public async Task<IActionResult> Register()
        {
          
           return await Task.Run(() => { return View(); });
        }
        public async Task<IActionResult> CreateVerifyCode(string re,string lo)
        {

           string verifyCode = VerifyCode.RndNum(5);
            ////MemoryStream ms = VerifyCode.Create(out verifyCode, 4);
            //MemoryStream ms = ImageFactory.BuildImage(verifyCode, 60, 100, 20);
            //TempData["verifyCode"] = verifyCode;
            ////HttpContext.Session.SetString("verifyCode", verifyCode);
            ////string web_path = webHostEnvironment.WebRootPath;
            ////FileStream ms = new FileStream(web_path+"/image/user.png", FileMode.Open);
            //return File(ms, @"image/jpeg");

            //string code = VerifyCode.GetSingleObj().CreateVerifyCode(VerifyCode.VerifyCodeType.MixVerifyCode);

            return await Task.Run(() =>
            {
                var bitmap = VerifyCode.GetSingleObj().CreateBitmapByImgVerifyCode(verifyCode, 100, 40);
                
                if (!string.IsNullOrWhiteSpace(re))
                {
                    TempData["REverifyCode"] = verifyCode.ToLower();
                }
                else if(!string.IsNullOrWhiteSpace(lo))
                {
                    TempData["LOverifyCode"] = verifyCode.ToLower();
                }
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Jpeg);
                return File(stream.ToArray(), "image/jpeg");
            });
        }
        //[HttpPost]
        //public IActionResult LoginCheck(UserLoginViewModel model)
        //{
        //    if (TempData["LOverifyCode"].ToString() == model.VerfiyCode)
        //    {
        //        LoginDTO loginDTO =  userService.LoginPassWordAndEmailAsync(model.PassWord, model.Email).Result;
        //        AjaxResult ajaxResult = new AjaxResult();
        //        if (loginDTO.IsLogin == false)
        //        {
        //            ajaxResult.Status = "error";
        //            ajaxResult.ErrorMsg = loginDTO.Msg;
        //            return Json(ajaxResult);
        //        }
        //        HttpContext.Session.SetString("UserId", loginDTO.Id.ToString());
        //      string userid=  HttpContext.Session.GetString("UserId");
        //        return Json(new AjaxResult { Status = "ok", ErrorMsg = "登录成功" });
        //    }
        //    else
        //    {
        //        return Json(new AjaxResult { Status = "error", ErrorMsg = "验证码错误" });
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> LoginCheckAsync(UserLoginViewModel model)
        {
            if (TempData["LOverifyCode"].ToString() == model.VerfiyCode)
            {
                LoginDTO loginDTO = await userService.LoginPassWordAndEmailAsync(model.PassWord, model.Email);
                AjaxResult ajaxResult = new AjaxResult();
                if (loginDTO.IsLogin == false)
                {
                    ajaxResult.Status = "error";
                    ajaxResult.ErrorMsg = loginDTO.Msg;
                    return Json(ajaxResult);
                }
                HttpContext.Session.SetString("UserId", loginDTO.Id.ToString());
                return Json(new AjaxResult { Status = "ok", ErrorMsg = "登录成功" });
            }
            else
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "验证码错误" });
            }
        }
        public IActionResult Test2()
        {
            return Content("hello");
        }
            public IActionResult Test1()
        {
            return Redirect("/Main/Index");
        }
      
        public async Task<IActionResult> RegisterCheck(UserRegisterModel model)
        {
            if (TempData["REverifyCode"].ToString()==model.verfiyCode)
            {
                if (!ModelState.IsValid)
                {
                    return Json(new AjaxResult() { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
                }
                if (await RedisHelper.GetRedisDatabase().SetContainsAsync("EmailAll", model.Email))
                {
                    return Json(new AjaxResult() { Status = "error", ErrorMsg = "email is exists" });
                }
                await RedisHelper.GetRedisDatabase().HashSetAsync("Email" + model.Email, "Name", model.Name);
                await RedisHelper.GetRedisDatabase().HashSetAsync("Email" + model.Email, "Email", model.Email);
                await RedisHelper.GetRedisDatabase().HashSetAsync("Email" + model.Email, "PassWord", model.PassWord);
                await RedisHelper.GetRedisDatabase().HashSetAsync("Email" + model.Email, "Guid", Guid.NewGuid().ToString());

                await RedisHelper.GetRedisDatabase().ListLeftPushAsync("Email", model.Email);
                
                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                return Json(new AjaxResult { Status = "error",ErrorMsg="验证码错误" });
            }
        }
       
        [HttpGet]
         public async Task<IActionResult> emailCheck(string Email,string Guid)
        {
            string guid = await RedisHelper.GetRedisDatabase().HashGetAsync("Email" + Email, "Guid");
            if (Guid==guid)
            {
                Model.User user = new Model.User();
                user.Email= await RedisHelper.GetRedisDatabase().HashGetAsync("Email" + Email, "Email");
                user.Name= await RedisHelper.GetRedisDatabase().HashGetAsync("Email" + Email, "Name");
                user.PassWord= await RedisHelper.GetRedisDatabase().HashGetAsync("Email" + Email, "PassWord");

                if (!await userService.IsExists(user.Name))
                {
                    long id = await userService.AddNew(user);
                    HttpContext.Session.SetString("UserId", id.ToString());

                    await RedisHelper.GetRedisDatabase().SetAddAsync(id.ToString(), id);
                    await RedisHelper.GetRedisDatabase().SortedSetAddAsync("PicScore", id, 1);
                    return RedirectToAction("Index");
                }
            }
            return Json(new AjaxResult() { Status = "error", ErrorMsg = "guid error" });
        } 

       
    }
}