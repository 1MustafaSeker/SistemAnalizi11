using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class WorkingHours
    {
        public int Id { get; set; }
        public int? BarberShopId { get; set; }
        public int? Day { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public virtual BarberShop? BarberShop { get; set; }


     
    }
}
