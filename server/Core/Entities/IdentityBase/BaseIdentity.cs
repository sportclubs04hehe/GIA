using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
