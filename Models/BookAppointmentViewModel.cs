using EntityLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemAnalizi.Models
{
    public class BookAppointmentViewModel
    {
        [Required]
        public int BarberShopId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string Time { get; set; }

        [Required]
        public List<int> SelectedServices { get; set; }

        public int TotalPrice { get; set; }

        public List<WorkingHours> WorkingHours { get; set; }
        public List<BarberService> Services { get; set; }
        public List<string> AvailableTimeSlots { get; set; }
    }
}
