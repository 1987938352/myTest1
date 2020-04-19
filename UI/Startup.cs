using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common;
using IService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using UEditor.Core;
using UI.Filters;

namespace UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddUEditorService();
            services.AddSession();
            services.AddControllersWithViews(
                s =>
                {
                    s.Filters.Add(typeof(AuthorFilter));
                }
                );

            var serviceAsm = Assembly.Load(new AssemblyName("Service"));
            foreach (Type serviceType in serviceAsm.GetTypes().Where(t => typeof(ISupportService).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract))
            {
                var interfaceTypes = serviceType.GetInterfaces();
                foreach (var interfaceType in interfaceTypes)
                {
                    services.AddSingleton(interfaceType, serviceType);
                }
            }

            ThreadPool.QueueUserWorkItem(async (a) =>
            {
                while (true)
                {
                    if (!string.IsNullOrWhiteSpace(RedisHelper.GetRedisDatabase().ListGetByIndex("Email", 0)))
                    {
                        string Email = await RedisHelper.GetRedisDatabase().ListRightPopAsync("Email");
                        string EGuid = await RedisHelper.GetRedisDatabase().HashGetAsync("Email" + Email, "Guid");
                    if (EGuid != null)
                        {
                            await PostEmail.EmailPost(Email,$"http://5001/Main/emailCheck?Email={Email}&&Guid={EGuid}");
                        }
                       
                    }
                    else if (!string.IsNullOrWhiteSpace(RedisHelper.GetRedisDatabase().ListGetByIndex("ExceptionLog", 0)))
                    {
                        LoggerHelper.Warn(RedisHelper.GetRedisDatabase().ListRightPop("ExceptionLog"));
                    }
                    else
                    {
                        Thread.Sleep(10000);
                    }
                }

            });
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "127.0.0.1";

            });
           
            //添加session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60); //session活期时间
                options.Cookie.HttpOnly = true;//设为httponly
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
