using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SistemAnalizi.Controllers
{
    public class BarberServiceController : Controller
    {
        private readonly ILogger<BarberServiceController> _logger;
        private readonly BarberServiceManager _barberServiceManager;
        private readonly OperationManager _operationManager;
        private readonly Context _context;

        public BarberServiceController(ILogger<BarberServiceController> logger)
        {
            _logger = logger;
            _barberServiceManager = new BarberServiceManager(new EfBarberService());
            _operationManager = new OperationManager(new EfOperationDal());
            _context = new Context();
        }

        public IActionResult Index()
        {
            var userName = User.Identity.Name;
            var userId = _context.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();
            var barberShopId = _context.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();
            var values = _context.BarberServices
                .Include(bs => bs.Operation)
                .Where(x => x.BarberShopId == barberShopId)
                .ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userName = User.Identity.Name;
            var userId = _context.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();
            var barberShopId = _context.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();

            // Operation listesini Include ile çekelim
            var operations = _context.Operations.Select(o => new
            {
                o.Id,
                o.Name,
                o.MinPrice,
                o.MaxPrice
            }).ToList();

            ViewBag.Operations = operations;
            ViewBag.OperationsList = new SelectList(operations, "Id", "Name");

            return View(new BarberService { BarberShopId = barberShopId });
        }

        [HttpPost]
        public IActionResult Create(BarberService barberService)
        {
            try
            {
                var userName = User.Identity.Name;
                var userId = _context.Users.Where(x => x.UserName == userName).Select(y => y.Id).FirstOrDefault();
                var barberShopId = _context.BarberShops.Where(x => x.UserId == userId).Select(x => x.Id).FirstOrDefault();
                barberService.BarberShopId = barberShopId;

                // Aynı hizmetin daha önce eklenip eklenmediğini kontrol et
                var existingService = _context.BarberServices
                    .FirstOrDefault(bs => bs.BarberShopId == barberShopId && 
                                         bs.OperationId == barberService.OperationId);

                if (existingService != null)
                {
                    ModelState.AddModelError("OperationId", "Bu hizmet zaten eklenmiş.");
                    return ReloadCreateView(barberService);
                }

                var operation = _operationManager.GetById(barberService.OperationId.Value);
                if (operation == null)
                {
                    ModelState.AddModelError("OperationId", "Invalid operation selected.");
                    return ReloadCreateView(barberService);
                }

                if (barberService.Price < operation.MinPrice || barberService.Price > operation.MaxPrice)
                {
                    ModelState.AddModelError("Price", $"Fiyat, {operation.MinPrice} ile {operation.MaxPrice} arasında olmalıdır.");
                    return ReloadCreateView(barberService);
                }

                _barberServiceManager.TAdd(barberService);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding BarberService");
                ModelState.AddModelError("", "Hizmet eklenirken bir hata oluştu.");
                return ReloadCreateView(barberService);
            }
        }

        private IActionResult ReloadCreateView(BarberService barberService)
        {
            var operations = _context.Operations.Select(o => new
            {
                o.Id,
                o.Name,
                o.MinPrice,
                o.MaxPrice
            }).ToList();

            ViewBag.Operations = operations;
            ViewBag.OperationsList = new SelectList(operations, "Id", "Name");
            return View(barberService);
        }

        // GET: BarberService/Edit/5
        public IActionResult Edit(int id)
        {
            var barberService = _context.BarberServices.FirstOrDefault(x => x.Id == id); // Context'ten al
            if (barberService == null)
            {
                return NotFound();
            }

            var operations = _context.Operations.Select(o => new
            {
                o.Id,
                o.Name,
                o.MinPrice,
                o.MaxPrice
            }).ToList();

            ViewBag.Operations = operations;
            ViewBag.OperationsList = new SelectList(operations, "Id", "Name");

            return View(barberService);
        }

        // POST: BarberService/Edit/5
        [HttpPost]
        public IActionResult Edit(BarberService barberService)
        {
            try
            {
                var existingService = _context.BarberServices
                    .Include(bs => bs.Operation)
                    .Include(bs => bs.BarberShop)
                    .Include(bs => bs.Appointments)
                    .FirstOrDefault(bs => bs.Id == barberService.Id);

                if (existingService == null)
                {
                    return NotFound();
                }

                var operation = _context.Operations.FirstOrDefault(o => o.Id == existingService.OperationId);
                if (operation == null)
                {
                    return NotFound();
                }

                if (barberService.Price < operation.MinPrice || barberService.Price > operation.MaxPrice)
                {
                    ModelState.AddModelError("Price", $"Fiyat, {operation.MinPrice} ile {operation.MaxPrice} arasında olmalıdır.");
                    return ReloadEditView(barberService);
                }

                // Sadece price'ı güncelle, diğer alanları koru
                existingService.Price = barberService.Price;
                _context.SaveChanges();

                TempData["Success"] = "Hizmet fiyatı başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating BarberService");
                ModelState.AddModelError("", "Hizmet güncellenirken bir hata oluştu.");
                return ReloadEditView(barberService);
            }
        }

        private IActionResult ReloadEditView(BarberService barberService)
        {
            var operations = _context.Operations.Select(o => new
            {
                o.Id,
                o.Name,
                o.MinPrice,
                o.MaxPrice
            }).ToList();

            ViewBag.Operations = operations;
            ViewBag.OperationsList = new SelectList(operations, "Id", "Name");
            return View(barberService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // İlgili hizmeti ve randevuları getir
                        var barberService = _context.BarberServices
                            .Include(bs => bs.Appointments)
                            .FirstOrDefault(bs => bs.Id == id);

                        if (barberService == null)
                        {
                            return NotFound();
                        }

                        // Önce ilişkili randevuları sil
                        if (barberService.Appointments != null && barberService.Appointments.Any())
                        {
                            _context.Appointments.RemoveRange(barberService.Appointments);
                        }

                        // Sonra hizmeti sil
                        _context.BarberServices.Remove(barberService);
                        _context.SaveChanges();
                        transaction.Commit();

                        TempData["Success"] = "Hizmet ve ilişkili randevular başarıyla silindi.";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Hizmet silinirken hata oluştu");
                        TempData["Error"] = "Hizmet silinirken bir hata oluştu.";
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet silinirken hata oluştu");
                TempData["Error"] = "Hizmet silinirken bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }
    }
}

