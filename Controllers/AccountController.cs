// ...existing code...

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

// ...existing code...
