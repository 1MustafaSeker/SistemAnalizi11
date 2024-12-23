using EntityLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemAnalizi.Models
{

    public class BookAppointmentViewModel
    {
        [Required(ErrorMessage = "Berber seçimi zorunludur")]
        public int BarberShopId { get; set; }

        public string BarberShopName { get; set; } = string.Empty; // Remove validation and set default value

        [Required(ErrorMessage = "Tarih seçimi zorunludur")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Saat seçimi zorunludur")]
        public string AppointmentTime { get; set; }

        [Required(ErrorMessage = "Hizmet seçimi zorunludur")]
        public int BarberServiceId { get; set; }

        [Required]
        public int UserId { get; set; }
        public int Status { get; set; }
        public bool IsCompleted { get; set; }
        public int TotalPrice { get; set; }

        public List<BarberServiceViewModel> BarberServices { get; set; } = new();
        public List<string> AvailableTimeSlots { get; set; } = new();
    }

    public class BarberServiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
