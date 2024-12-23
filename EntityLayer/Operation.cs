using System;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer
{
    public class Operation
    {
        
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        
        public virtual ICollection<BarberService>? BarberServices { get; set; }
    }
}
