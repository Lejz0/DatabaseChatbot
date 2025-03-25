using Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain
{
    public class Database : BaseEntity
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database_Name { get; set; }

        public string? OwnerId { get; set; }

        public ChatApplicationUser? User { get; set; }

        public ICollection<Question>? Questions { get; set; }
    }
}
