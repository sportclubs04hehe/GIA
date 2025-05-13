using System.ComponentModel.DataAnnotations;

namespace Core.Entities.IdentityBase
{
    public class BaseIdentity
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsDelete { get; set; }
        public string? CreatedBy { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
    }
}
