using System.ComponentModel.DataAnnotations;

namespace Domain.Domain
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
