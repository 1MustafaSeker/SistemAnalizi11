using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EntityLayer;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayer.Concrete;
using Microsoft.EntityFrameworkCore;

namespace SistemAnalizi.Controllers
{
    
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Context _context;

        public UserController(UserManager<AppUser> userManager, Context context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                // Delete related transactions
                var appointments = _context.Appointments.Where(a => a.UserId == id).ToList();
                foreach (var appointment in appointments)
                {
                    var transactions = _context.Transactions.Where(t => t.AppointmentId == appointment.Id).ToList();
                    _context.Transactions.RemoveRange(transactions);
                }

                // Delete related appointments
                _context.Appointments.RemoveRange(appointments);

                // Delete related barber shops
                var barberShops = _context.BarberShops.Where(bs => bs.UserId == id).ToList();
                _context.BarberShops.RemoveRange(barberShops);

                // Save changes to delete related records
                await _context.SaveChangesAsync();

                // Delete the user
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
