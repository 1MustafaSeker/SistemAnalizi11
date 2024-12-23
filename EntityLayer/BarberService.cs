using System.Collections.Generic;

namespace EntityLayer
{
    public class BarberService
    {
        public int Id { get; set; }
        public int? BarberShopId { get; set; }
        public int? OperationId { get; set; }
        public int? Price { get; set; }

        public virtual BarberShop BarberShop { get; set; }
        public virtual Operation Operation { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } // Many-to-many relationship
    }
}




