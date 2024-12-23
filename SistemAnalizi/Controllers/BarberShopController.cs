using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemAnalizi.Models;
using System.Security.Claims;

namespace SistemAnalizi.Controllers
{
    public class BarberShopController : Controller
    {
        BarberShopManager barberShopManager = new BarberShopManager(new EfBarberShopDal());
        BarberServiceManager barberServiceManager = new BarberServiceManager(new EfBarberService());
        WorkingHoursManager workingHoursManager = new WorkingHoursManager(new EfWorkingHoursDal());
        Context c = new Context();
        AppointmentManager appointmentManager = new AppointmentManager(new EfAppointmentDal());

        [AllowAnonymous]
        public IActionResult Index()
        {
            var values = barberShopManager.GetList();
            return View(values); // Ensure this returns a list of BarberShop objects
        }

        public IActionResult Details(int id)
        {
            ViewBag.i = id;
            var values = c.BarberShops
                .Include(bs => bs.BarberServices)
                .ThenInclude(bs => bs.Operation)
                .Include(bs => bs.WorkingHours)
                .FirstOrDefault(bs => bs.Id == id);
            return View(values);
        }
    }
}
