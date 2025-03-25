using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<ChatApplicationUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<ChatApplicationUser>();
        }
        public ChatApplicationUser Get(string id)
        {
            return entities
                .Include(s => s.Databases)
                .Include("Databases.Questions")
                .SingleOrDefault(s => s.Id == id);
        }

        public IEnumerable<ChatApplicationUser> GetAll()
        {
            return entities
                .Include("Databases")
                .AsEnumerable(); 
        }

        public void Insert(ChatApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(ChatApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(ChatApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entities.Remove(entity);
            context.SaveChanges();
        }

    }
}
