using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Concrete;
using System.Linq;

namespace SistemAnalizi.Controllers
{
    public class TransactionController : Controller
    {
        TransactionManager transactionManager = new TransactionManager(new EfTransactionDal());
        Context c = new Context();

        public IActionResult Index()
        {
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();

            // Filter transactions based on the appointment's UserId
            var transactions = c.Transactions
                .Include(t => t.Appointment)
                    .ThenInclude(a => a.BarberService)
                        .ThenInclude(bs => bs.Operation)
                .Include(t => t.Appointment)
                    .ThenInclude(a => a.BarberShop)
                .Where(t => t.Appointment.UserId == userId)
                .ToList();

            return View(transactions);
        }

        public IActionResult ListByBarberShop()
        {
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();

            // Get the BarberShopId
            var barberShopId = c.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();

            // Filter transactions based on the BarberShopId
            var transactions = c.Transactions
                .Include(t => t.Appointment)
                    .ThenInclude(a => a.BarberService)
                        .ThenInclude(bs => bs.Operation)
                .Include(t => t.Appointment)
                    .ThenInclude(a => a.BarberShop)
                .Where(t => t.Appointment.BarberShopId == barberShopId)
                .ToList();

            return View(transactions);
        }
    }
}
