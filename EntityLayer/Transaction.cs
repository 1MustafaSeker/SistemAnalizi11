using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class Transaction
    {
        public int Id { get; set; }

        public int? AppointmentId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        
        public PaymentStatus? Status { get; set; }

        public virtual Appointment? Appointment { get; set; }

        public enum PaymentStatus
        {
            Onaylandı,
            İptal
            
        }
    }
}
