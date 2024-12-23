namespace SistemAnalizi.Models
{
    public class RegisterBarberViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; } // Yeni alan
        public string Surname { get; set; } // Yeni alan
        public string CitizenshipNumber { get; set; } // Yeni alan
        public string BarberShopName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string District { get; set; }
        public string Type { get; set; }
    }
}
