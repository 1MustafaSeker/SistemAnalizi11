using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace EntityLayer
{
    public class AppUser : IdentityUser<int>
    {
        public string? Name { get; set; }
        public string? CitizenshipNumber{ get; set; }
        public string? Surname { get; set; }
        public virtual ICollection<Appointment>? Appointments { get; set; }
        public virtual ICollection<BarberShop> BarberShops { get; set; }
    }
}
