using IService;
using Model;
using ModelDTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using StackExchange.Redis;
using Common;

namespace Service
{
  
    public class UserService : IUserService
    {
        public async Task<List<UserDTO>> GetAllAsync()
        {
            using (MyDbContext ctx=new MyDbContext())
            {
                List<int> a = new List<int>();
                var list = await ctx.User.Where(u => u.IsDelete == false).Select(u => new UserDTO { Id=u.Id,Name=u.Name,Email=u.Email}).ToListAsync();
                return list;
            }
        }

        /// <summary>
        /// test
        /// </summary>
        /// <returns></returns>
        public List<UserDTO> getUserDtosBySql()
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                var list=  ctx.User.FromSqlRaw("select * from t_user").ToList();
                var  list2 = list.Select(a => ToDTO(a)).ToList();
                return list2;
            }
        }
        /// <summary>
        /// test
        /// </summary>
        /// <returns></returns>
        public User getUserDtosByNOLazy()
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                var list = ctx.User.Where(e => e.IsDelete == false);
                User user = list.Where(e => e.Id == 1).FirstOrDefault();
                return user;
            }
        }
        public UserDTO ToDTO(User user)
        {
            UserDTO userDTO = new UserDTO();
            userDTO.Id = user.Id;
            userDTO.Name = user.Name;
            userDTO.PassWord = user.PassWord;
            userDTO.Email = user.Email;
            return userDTO;
        }
        public async Task<bool>IsExists(string Email)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                bool exists = await ctx.User.AnyAsync(u => u.Email == Email);
                return exists;
            }
        }
        public async Task<long> AddNew(User userNew)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                BaseService<User> bs = new BaseService<User>(ctx);
                bool exists = await bs.GetAll().AnyAsync(u => u.Email == userNew.Email);
                if (exists)
                {
                    await RedisHelper.GetRedisDatabase().SetAddAsync("EmailAll", userNew.Email);
                 var tran=   RedisHelper.GetRedisDatabase().CreateTransaction();
                   
                    throw new ArgumentException("手机号已经存在");
                }
                ctx.User.Add(userNew);
               await ctx.SaveChangesAsync();
                return userNew.Id;
            }
        }
        public async Task<bool> SerRedisAsync(string str)
        {
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                IDatabase db = redis.GetDatabase();
             return  await  db.StringSetAsync("huhu", str);
            }
        }
        public async Task<LoginDTO> LoginPassWordAndEmailAsync(string passWord, string email)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                var user = await ctx.User.Where(u => u.Email == email&&u.IsDelete==false).SingleOrDefaultAsync();
                LoginDTO loginDTO = new LoginDTO();
                if (user==null)
                {
                    loginDTO.IsLogin = false;
                    loginDTO.Id = -1;
                    loginDTO.Msg = "Email不存在";
                }
                else if (user.PassWord!=passWord)
                {
                    loginDTO.IsLogin = false;
                    loginDTO.Id = -1;
                    loginDTO.Msg = "Email或者密码错误";
                }
                else
                {
                    loginDTO.IsLogin = true;
                    loginDTO.Id = user.Id;
                    loginDTO.Msg = "";
                }
                return loginDTO;
            }
        }
        public async Task<UserDTO[]> GetAllUserAsync(int start,int size)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                var users = await ctx.User.Where(u => u.Id != 1 && u.IsDelete == false).OrderBy(u=>u.Id).Skip(start).Take(size).AsNoTracking().Select(u => new UserDTO { Name = u.Name, Id = u.Id, Email = u.Email, PassWord = u.PassWord }).ToArrayAsync();
                return users;
            }
        }
        public async Task<bool> DeleteByIdAsync(long Id)
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                var user = await ctx.User.SingleOrDefaultAsync(u => u.Id == Id);
                if (user==null)
                {
                    return false;
                }
                user.IsDelete = true;
               await ctx.SaveChangesAsync();
                return true;
            }
        }
        public async Task<long> GetAllCountAsync()
        {
            using (MyDbContext ctx = new MyDbContext())
            {
                var count = await ctx.User.Where(u => u.Id != 1 && u.IsDelete == false).LongCountAsync();
                return count;
            }
        }
        public bool HasRole(long Id,string RoleName)
        {
            using (MyDbContext ctx=new MyDbContext())
            {
                var user =  ctx.User.Where(u => u.Id == Id && u.IsDelete == false).AsNoTracking().SingleOrDefault();
                if (user==null)
                {
                    throw new ArgumentException("找不到id=" + Id + "的用户");
                }
                var role =  ctx.Role.FirstOrDefault(r => r.Name == RoleName && r.IsDelete == false);
                if (role!=null)
                {
                    if (ctx.UserRole.Any(u => u.UserId == role.Id&&u.RoleId==role.Id))
                    {
                        return true;
                    }
                }
              
                return false;
            }
        }
    }
}
