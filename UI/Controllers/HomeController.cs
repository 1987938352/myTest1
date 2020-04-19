using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using ModelDTO;
using Service;
using UI.Models;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            UserService userService = new UserService();
            List<UserDTO> list = await userService.GetAllAsync();
            return View(list);
        }
        /// <summary>
        /// 测试用
        /// </summary>
        /// <returns></returns>
        public IActionResult Index2()
        {
            UserService userService = new UserService();
            List<UserDTO> list = userService.getUserDtosBySql();
            return View(list);
        }
        public IActionResult Index3()
        {
            UserService userService = new UserService();
            User user = userService.getUserDtosByNOLazy();
            return View(user);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
