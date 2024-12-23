using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemAnalizi.Models;
using System.Security.Claims;

namespace SistemAnalizi.Controllers
{
    public class AppointmentController : Controller
    {
        BarberServiceManager barberServiceManager = new BarberServiceManager(new EfBarberService());
        AppointmentManager appointmentManager = new AppointmentManager(new EfAppointmentDal());
        Context c = new Context();

        public IActionResult CustomerAppointmentIndex()
        {
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();
            var values = c.Appointments
                .Include(a => a.User)
                .Include(a => a.BarberShop)
                .Include(a => a.BarberService)
                .ThenInclude(bs => bs.Operation)
                .Where(x => x.UserId == userId);
            return View(values);
        }

        public IActionResult BarberAppointmentIndex()
        {
            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();
            var barberShopId = c.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();
            var values = c.Appointments
                .Include(a => a.User)
                .Include(a => a.BarberService)
                .ThenInclude(bs => bs.Operation)
                .Where(x => x.BarberShopId == barberShopId);
            return View(values);
        }

        [HttpGet]
        public IActionResult BookAppointment(int id, DateTime? appointmentDate = null)
        {
            var barberShop = c.BarberShops
                .Include(bs => bs.WorkingHours)
                .Include(bs => bs.BarberServices)
                    .ThenInclude(bs => bs.Operation)
                .AsNoTracking()
                .FirstOrDefault(bs => bs.Id == id);

            if (barberShop == null)
            {
                return RedirectToAction("Index", "BarberShop");
            }

            var selectedDate = appointmentDate ?? DateTime.Today;
            var dayOfWeek = (int)selectedDate.DayOfWeek;

            var workingHour = barberShop.WorkingHours?.FirstOrDefault(wh => wh.Day == dayOfWeek);
            var availableTimeSlots = new List<string>();

            if (workingHour != null)
            {
                TimeSpan start = workingHour.OpenTime;
                TimeSpan end = workingHour.CloseTime;

                // Get already booked time slots for the selected date
                var bookedTimeSlots = c.Appointments
                    .Where(a => a.BarberShopId == id && a.AppointmentDate == selectedDate)
                    .Select(a => a.AppointmentTime)
                    .ToList();

                while (start < end)
                {
                    if (!bookedTimeSlots.Contains(start))
                    {
                        availableTimeSlots.Add(start.ToString(@"hh\:mm"));
                    }
                    start = start.Add(TimeSpan.FromHours(1));
                }
            }

            var userName = User.Identity.Name;
            var userId = c.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();

            var services = barberShop.BarberServices?
                .Where(bs => bs.Operation != null)
                .Select(bs => new BarberServiceViewModel
                {
                    Id = bs.Id,
                    Name = bs.Operation.Name,
                    Price = bs.Price ?? 0
                })
                .ToList() ?? new List<BarberServiceViewModel>();

            var model = new BookAppointmentViewModel
            {
                BarberShopId = id,
                AppointmentDate = selectedDate,
                BarberServices = services,
                AvailableTimeSlots = availableTimeSlots,
                UserId = userId,
                Status = 0,
                IsCompleted = false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BookAppointment(BookAppointmentViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Where(e => !e.ErrorMessage.Contains("BarberShopName")) // Ignore BarberShopName errors
                        .Select(e => e.ErrorMessage);
                    
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return ReloadBookAppointmentView(model);
                }

                if (model.UserId == 0)
                {
                    ModelState.AddModelError("", "Lütfen giriş yapınız.");
                    return ReloadBookAppointmentView(model);
                }

                var selectedService = c.BarberServices
                    .Include(bs => bs.Operation)
                    .FirstOrDefault(bs => bs.Id == model.BarberServiceId);

                if (selectedService == null)
                {
                    ModelState.AddModelError("", "Lütfen bir hizmet seçiniz.");
                    return ReloadBookAppointmentView(model);
                }

                var appointment = new Appointment
                {
                    AppointmentDate = model.AppointmentDate.Date,
                    AppointmentTime = TimeSpan.Parse(model.AppointmentTime),
                    BarberShopId = model.BarberShopId,
                    BarberServiceId = model.BarberServiceId,
                    UserId = model.UserId,
                    Status = 0, // 0 represents pending in most status enums
                    TotalPrice = selectedService.Price ?? 0,
                    IsCompleted = false
                };

                appointmentManager.TAdd(appointment);
                TempData["Success"] = "Randevu başarıyla oluşturuldu.";
                return RedirectToAction("CustomerAppointmentIndex");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Randevu oluşturulurken hata: {ex.Message}");
                return ReloadBookAppointmentView(model);
            }
        }

        [HttpPost]
        public IActionResult UpdateAppointmentStatus(int appointmentId, int status, decimal amount)
        {
            using (var transaction = c.Database.BeginTransaction())
            {
                try
                {
                    var appointment = c.Appointments.SingleOrDefault(a => a.Id == appointmentId);
                    if (appointment != null)
                    {
                        // Calculate transaction amount based on status
                        decimal transactionAmount;
                        if (status == 2) // Pending to Processing
                        {
                            transactionAmount = Math.Round(amount * 0.30m, 2); // 30% deposit
                        }
                        else if (status == 1) // Processing to Completed
                        {
                            transactionAmount = amount; // Full amount
                        }
                        else
                        {
                            transactionAmount = 0; // For cancelled or other statuses
                        }

                        // Update appointment status
                        appointment.Status = status;
                        c.Update(appointment);
                        c.SaveChanges();

                        // Create transaction record
                        var newTransaction = new Transaction
                        {
                            AppointmentId = appointmentId,
                            Amount = transactionAmount,
                            TransactionDate = DateTime.Now,
                            Status = status == 1 ? Transaction.PaymentStatus.Onaylandı : Transaction.PaymentStatus.İptal
                        };

                        c.Transactions.Add(newTransaction);
                        c.SaveChanges();

                        transaction.Commit();
                        string message = status == 1 
                            ? $"Randevu onaylandı. Toplam tutar: {transactionAmount:C}" 
                            : (status == 2 
                                ? $"Randevu işleme alındı. Depozito tutarı: {transactionAmount:C}"
                                : "Randevu iptal edildi.");
                                
                        return Json(new { success = true, message = message });
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Randevu bulunamadı." });
        }

        private IActionResult ReloadBookAppointmentView(BookAppointmentViewModel model)
        {
            var barberShop = c.BarberShops
                .Include(bs => bs.WorkingHours)
                .Include(bs => bs.BarberServices)
                    .ThenInclude(bs => bs.Operation)
                .FirstOrDefault(bs => bs.Id == model.BarberShopId);

            if (barberShop != null)
            {
                var dayOfWeek = (int)model.AppointmentDate.DayOfWeek;
                var workingHour = barberShop.WorkingHours?.FirstOrDefault(wh => wh.Day == dayOfWeek);
                
                model.AvailableTimeSlots = GenerateTimeSlots(workingHour);
                model.BarberServices = GetBarberServices(barberShop);
            }

            return View(model);
        }

        private List<string> GenerateTimeSlots(WorkingHours workingHour)
        {
            var slots = new List<string>();
            if (workingHour != null)
            {
                var currentHour = workingHour.OpenTime;
                while (currentHour < workingHour.CloseTime)
                {
                    slots.Add(currentHour.ToString(@"hh\:mm"));
                    currentHour = currentHour.Add(TimeSpan.FromHours(1));
                }
            }
            return slots;
        }

        private List<BarberServiceViewModel> GetBarberServices(BarberShop barberShop)
        {
            return barberShop.BarberServices?
                .Where(bs => bs.Operation != null)
                .Select(bs => new BarberServiceViewModel
                {
                    Id = bs.Id,
                    Name = bs.Operation.Name ?? "Unnamed Service",
                    Price = bs.Price ?? 0
                })
                .ToList() ?? new List<BarberServiceViewModel>();
        }
    }
}
