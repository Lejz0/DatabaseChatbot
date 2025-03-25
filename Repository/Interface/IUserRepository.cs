using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<ChatApplicationUser> GetAll();
        ChatApplicationUser Get(string id);

        void Insert(ChatApplicationUser entity);
        void Update(ChatApplicationUser entity);
        void Delete(ChatApplicationUser entity);
    }
}
