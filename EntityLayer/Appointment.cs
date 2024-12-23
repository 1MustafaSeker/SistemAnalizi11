using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int? BarberShopId { get; set; }
        public int? BarberServiceId { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public int Status { get; set; }
        public int TotalPrice { get; set; }

        public virtual BarberShop BarberShop { get; set; }
        public virtual AppUser User { get; set; }
        public virtual BarberService BarberService { get; set; } 
    }

    public enum AppointmentStatus
    {
        Beklemede,
        Tamamlandi,
        iptal
        
    }
}
