using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;

namespace SistemAnalizi.Controllers
{
    public class WorkingHoursController : Controller
    {
        private readonly WorkingHoursManager workingHoursManager = new WorkingHoursManager(new EfWorkingHoursDal());
        private readonly Context c = new Context();

        // GET: WorkingHours
        public IActionResult Index()
        {
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();

            // barberShopId'yi al
            var barberShopId = c.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();

            // İlgili çalışma saatlerini al
            var workingHoursList = c.WorkingHours.Where(x => x.BarberShopId == barberShopId).ToList();
            return View(workingHoursList);
        }

        // GET: WorkingHours/Create
        public IActionResult Create()
        {
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();

            // barberShopId'yi al
            var barberShopId = c.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();

            ViewBag.BarberShopId = barberShopId; // BarberShopId'yi ViewBag'e ekle
            return View(); // Yeni çalışma saatleri oluşturma görünümünü döndür
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(WorkingHours workingHours)
        {
            // Kullanıcının kimliğini al
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();

            // barberShopId'yi al
            var barberShopId = c.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();
            workingHours.BarberShopId = barberShopId; // Çalışma saatlerine barberShopId ekle

            // Aynı günde kayıt kontrolü
            var existingRecord = c.WorkingHours
                .FirstOrDefault(x => x.BarberShopId == barberShopId && x.Day == workingHours.Day);

            if (existingRecord != null)
            {
                ModelState.AddModelError("Day", "Bu gün için zaten bir kayıt bulunmaktadır."); // Hata mesajı ekle
            }

            // Açılış saati kapanış saatinden büyük olmamalı
            if (workingHours.OpenTime >= workingHours.CloseTime)
            {
                ModelState.AddModelError("OpeningTime", "Açılış saati, kapanış saatinden daha büyük olamaz."); // Hata mesajı ekle
            }

            if (ModelState.IsValid)
            {
                workingHoursManager.TAdd(workingHours); // Yeni çalışma saatlerini ekle
                return RedirectToAction("Index"); // İşlemden sonra Index sayfasına yönlendir
            }

            return View(workingHours); // Hatalı durumlarda formu tekrar göster
        }

        // GET: WorkingHours/Edit/5
        public IActionResult Edit(int id)
        {
            var workingHours = workingHoursManager.GetById(id); // Mevcut çalışma saatlerini al
            if (workingHours == null)
            {
                return NotFound();
            }

            // barberShopId'yi al
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();
            var barberShopId = c.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();

            ViewBag.BarberShopId = barberShopId; // BarberShopId'yi ViewBag'e ekle
            return View(workingHours); // Düzenleme görünümünü döndür
        }

        // POST: WorkingHours/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(WorkingHours workingHours)
        {
            // Kullanıcının kimliğini al
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();

            // barberShopId'yi al
            var barberShopId = c.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();
            workingHours.BarberShopId = barberShopId; // Çalışma saatlerine barberShopId ekle

            // Aynı günde kayıt kontrolü
            var existingRecord = c.WorkingHours
                .FirstOrDefault(x => x.BarberShopId == barberShopId && x.Day == workingHours.Day && x.Id != workingHours.Id);

            if (existingRecord != null)
            {
                ModelState.AddModelError("Day", "Bu gün için zaten bir kayıt bulunmaktadır."); // Hata mesajı ekle
            }

            // Açılış saati kapanış saatinden büyük olmamalı
            if (workingHours.OpenTime >= workingHours.CloseTime)
            {
                ModelState.AddModelError("OpeningTime", "Açılış saati, kapanış saatinden daha büyük olamaz."); // Hata mesajı ekle
            }

            if (ModelState.IsValid)
            {
                workingHoursManager.TUpdate(workingHours); // Çalışma saatlerini güncelle
                return RedirectToAction("Index"); // İşlemden sonra Index sayfasına yönlendir
            }

            return View(workingHours); // Hatalı durumlarda formu tekrar göster
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var workingHours = workingHoursManager.GetById(id);
            if (workingHours == null)
            {
                return NotFound();
            }
            workingHoursManager.TDelete(workingHours); // Çalışma saatlerini sil
            return RedirectToAction("Index"); // Silme işleminden sonra Index sayfasına yönlendir
        }
    }
}