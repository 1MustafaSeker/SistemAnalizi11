using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class BarberShop
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? District { get; set; }
        public string? Type { get; set; }
        public string? Image { get; set; }
        public virtual AppUser? User { get; set; }
        public virtual ICollection<WorkingHours>? WorkingHours { get; set; }
        public virtual ICollection<BarberService>? BarberServices { get; set; }
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}
