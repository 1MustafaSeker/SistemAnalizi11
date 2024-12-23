using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using EntityLayer;

namespace SistemAnalizi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApprovalController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly BarberShopManager _barberShopManager;

        public ApprovalController(Context context, UserManager<AppUser> userManager, 
            BarberShopManager barberShopManager)
        {
            _context = context;
            _userManager = userManager;
            _barberShopManager = barberShopManager;
        }

        public IActionResult Index()
        {
            var pendingApprovals = _context.Approvals
                .Where(a => !a.IsApproved)
                .ToList();
            return View(pendingApprovals);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var approval = await _context.Approvals.FindAsync(id);
            if (approval == null)
                return NotFound();

            // Create user
            var user = new AppUser
            {
                UserName = approval.Email,
                Email = approval.Email,
                Name = approval.Name,
                Surname = approval.Surname,
                CitizenshipNumber = approval.CitizenshipNumber
            };

            var result = await _userManager.CreateAsync(user, approval.Password);
            if (result.Succeeded)
            {
                // Create barber shop
                var barberShop = new BarberShop
                {
                    Name = approval.BarberShopName,
                    Address = approval.Address,
                    Phone = approval.PhoneNumber,
                    UserId = user.Id,
                    Image = "Images/defaultbarber.png"
                };

                _barberShopManager.TAdd(barberShop);
                await _userManager.AddToRoleAsync(user, "Barber");

                // Update approval status
                approval.IsApproved = true;
                approval.ApprovalDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            return Json(new { success = false, errors = result.Errors });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var approval = await _context.Approvals.FindAsync(id);
            if (approval == null)
                return NotFound();

            _context.Approvals.Remove(approval);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
