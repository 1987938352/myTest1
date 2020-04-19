using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class BaseService<T> where T: Base
    {
        private MyDbContext ctx;
        public BaseService(MyDbContext ctx)
        {
            this.ctx = ctx;
        }
        public  IQueryable<T>  GetAll()
        {
            return  ctx.Set<T>().Where(e => e.IsDelete == false);
        } 
        public async Task<T> GetByidAsync(long id)
        {
            return await ctx.Set<T>().Where(e => e.Id == id).SingleOrDefaultAsync();
        }
        public async Task<IList<T>> GetByPageDateAsync(int startIndex,int count)
        {
            return await ctx.Set<T>().Where(e => e.IsDelete == false).OrderBy(e => e.Id).Skip((startIndex - 1) * count).Take(count).ToListAsync();
        }
        public async void MakeDeleteAsync(long id)
        {
            var data = await GetByidAsync(id);
            data.IsDelete = true;
            await  ctx.SaveChangesAsync();
        }
    }
}
