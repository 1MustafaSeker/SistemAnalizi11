using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EntityLayer; // Model sınıflarının bulunduğu namespace
using BusinessLayer.Abstract;
using SistemAnalizi.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authorization;

[AllowAnonymous]
public class RegisterController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    BarberShopManager barberShopManager = new BarberShopManager(new EfBarberShopDal());

    public RegisterController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
        
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
            // AppUser oluştur
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name, // Yeni alan
                Surname = model.Surname, // Yeni alan
                CitizenshipNumber = model.CitizenshipNumber // Yeni alan
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // BarberShop oluştur
                var barberShop = new BarberShop
                {
                    Name = model.BarberShopName,
                    Address = model.Address,
                    Phone = model.PhoneNumber,
                    UserId = user.Id,
                    Image="Images/defaultbarber.png"
                    
                    
                };

                barberShopManager.TAdd(barberShop); // Servis ile berber dükkanını ekle

                // Kullanıcıya rol atama (isteğe bağlı)
                await _userManager.AddToRoleAsync(user, "Barber");

                return RedirectToAction("Index", "Home"); // Başarılı kayıt sonrası yönlendirme
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return RedirectToAction("Login","Login");
    }










}