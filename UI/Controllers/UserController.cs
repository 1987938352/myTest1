using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Qiniu.Storage;
using UEditor.Core;
using UI.Attributes;
using UI.Models;

namespace UI.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration configuration;

        public UserController(IWebHostEnvironment webHostEnvironment,IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
        } 
        public IActionResult Index()
        {
            return View();
        }
        [CheckPermission("")]
        public IActionResult MyHome()
        {
            HttpContext.Session.SetString("UserId", 1.ToString());
            long UserId = Convert.ToInt64(HttpContext.Session.GetString("UserId"));

          string Url=  RedisHelper.GetRedisDatabase().StringGet($"{UserId}Pic");
            if (Url.IsNullOrWhiteSpace())
            {
                string tuming = configuration.GetValue<string>("MyQiniuQianZui");
                Url = $"{tuming}1";
            }

            return View(new QiNiuPicViewModel() { Url=Url,Id=UserId});
        }
        [CheckPermission("")]
        public async Task< IActionResult> OtherHome(long Id)
        {
        string  url= await  RedisHelper.GetRedisDatabase().StringGetAsync($"{Id}Pic");
            if (string.IsNullOrWhiteSpace(url))
            {
                string tuming = configuration.GetValue<string>("MyQiniuQianZui");
                url = $"{tuming}1";
            }
            return View(new QiNiuPicViewModel() { Url = url, Id = Id });
        }
        [CheckPermission("")]
        public async Task<IActionResult> TouPiao(long PicId)
        {
            long UserId = Convert.ToInt64(HttpContext.Session.GetString("UserId"));
            AjaxResult ajaxResult = new AjaxResult();
            if (await RedisHelper.GetRedisDatabase().SetContainsAsync(PicId.ToString(), UserId))
            {
                ajaxResult.Status = "error";
                ajaxResult.ErrorMsg = "已经点过了";
            }
            else
            {
              await  RedisHelper.GetRedisDatabase().SetAddAsync(PicId.ToString(), UserId);
                await RedisHelper.GetRedisDatabase().SortedSetIncrementAsync("PicScore", PicId, 1);
                ajaxResult.Status = "ok";
            }
            return Json(ajaxResult);
        }
        //public IActionResult UploadFile()
        //{
        //    if (Request.Form.Files.Count <= 0)
        //    {
        //        return Content("请选择图片");
        //    }
        //    string name = Request.Query["address"];
        //    string imgPath = "\\UploadFiles\\" + name + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + DateTime.Now.Day + "\\";
        //    string dicPath = webHostEnvironment.WebRootPath + imgPath;
        //    if (!Directory.Exists(dicPath))
        //    {
        //        Directory.CreateDirectory(dicPath);
        //    }
        //    var img = Request.Form.Files[0];
        //    if (img == null)
        //    {
        //        return Content("上传失败");
        //    }
        //    string ext = Path.GetExtension(img.FileName);
        //    //判断后缀是否是图片
        //    const string fileFilt = ".jpg|.jpeg|.png|";
        //    //判断后缀是否是图片
        //    if (ext == null)
        //    {
        //        return Content("上传的文件没有后缀");
        //    }
        //    if (fileFilt.IndexOf(ext.ToLower(), StringComparison.Ordinal) <= -1)
        //    {
        //        return Content("上传的文件不是图片");
        //    }
        //    string fileName = Guid.NewGuid().ToString() + ext;
        //    string filePath = Path.Combine(dicPath, fileName);
        //    using (FileStream fs = System.IO.File.Create(filePath))
        //    {
        //        img.CopyTo(fs);
        //        fs.Flush();
        //    }
        //    return Content(imgPath + fileName);
        //}
        //public IActionResult DeleteFile()
        //{
        //    return Content("delete");
        //}
        [HttpPost]
        public async Task<ActionResult> UploadPic()
        {
            var date = Request;
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);
            string webRootPath = webHostEnvironment.WebRootPath;
            string contentRootPath = webHostEnvironment.ContentRootPath;
            //foreach (var formFile in files)
            //{
            //这里我们就取第一张图片了‘’
            var formFile = files[0];
            Stream stream=  formFile.OpenReadStream();

            long UserId = Convert.ToInt64(HttpContext.Session.GetString("UserId"));
            string tuming= configuration.GetValue<string>("MyQiniuQianZui");
            RedisHelper.GetRedisDatabase().StringSet($"{UserId}Pic", $"{tuming}{UserId}");
            QiNiuAdd.UploadConfig(stream,$"{UserId}");
            return await Task.Run(() => { return Json(new AjaxResult { Status = "ok" }); }) ;
                //if (formFile.Length > 0)
                //{

                //    string fileExt =MVCHelper.GetFileExt(formFile.FileName); //文件扩展名，不含“.”
                //    long fileSize = formFile.Length; //获得文件大小，以字节为单位
                //    string newFileName = System.Guid.NewGuid().ToString() + "." + fileExt; //随机生成新的文件名
                //    var filePath = webRootPath + "/upload/" + newFileName;
                //PutPolicy putPolicy = new PutPolicy();
               
                //using (var stream = new FileStream(filePath, FileMode.Create))
                //    {

                //        await formFile.CopyToAsync(stream);
                //    }
                //}
            //}

            //return Json(new AjaxResult { Status = "ok" });
        }
    }
}