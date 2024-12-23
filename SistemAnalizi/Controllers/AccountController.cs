using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EntityLayer;
using SistemAnalizi.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayer.Concrete;

namespace SistemAnalizi.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        BarberShopManager barberShopManager = new BarberShopManager(new EfBarberShopDal());
        Context c = new Context();

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (returnUrl != null)
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult RegisterBarber()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterBarber(RegisterBarberViewModel model)
        {
            if (ModelState.IsValid)
            {
                var approval = new Approval
                {
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    CitizenshipNumber = model.CitizenshipNumber,
                    Password = model.Password,
                    BarberShopName = model.BarberShopName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    District = model.District, // Yeni eklenen alan
                    Type = model.Type, // Yeni eklenen alan
                    RequestDate = DateTime.Now,
                    IsApproved = false
                };

                c.Approvals.Add(approval);
                await c.SaveChangesAsync();

                TempData["Success"] = "Kayıt talebiniz alınmıştır. Onay sürecinden sonra bilgilendirileceksiniz.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    CitizenshipNumber = model.CitizenshipNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction("Login", "Account");
        }
    }
}
