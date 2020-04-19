using Model;
using ModelDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IService
{
    public interface IUserService:ISupportService
    {
        Task<List<UserDTO>> GetAllAsync();

        Task<long> AddNew(User user);
        Task<bool> SerRedisAsync(string str);
        Task<LoginDTO> LoginPassWordAndEmailAsync(string passWord, string email);
        bool HasRole(long Id, string RoleName);
        Task<UserDTO[]> GetAllUserAsync(int start, int size);
        Task<long> GetAllCountAsync();
        Task<bool> DeleteByIdAsync(long Id);
        Task<bool> IsExists(string Email);
    }
}
