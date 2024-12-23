using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemAnalizi.ViewComponents
{
    public class BarberServiceListByBarberShop : ViewComponent
    {
        BarberServiceManager barberServiceManager = new BarberServiceManager(new EfBarberService());
        Context c = new Context();

        public IViewComponentResult Invoke(int id)
        {
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();

            // barberShopId'yi al
            var barberShopId = c.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();

            // İlgili çalışma saatlerini al
            var workingHoursList = c.BarberServices
                .Include(bs => bs.Operation)
                .Where(x => x.BarberShopId == barberShopId)
                .ToList();

            return View(workingHoursList);
        }
    }
}
