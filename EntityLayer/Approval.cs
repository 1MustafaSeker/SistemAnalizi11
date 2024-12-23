namespace EntityLayer
{
    public class Approval
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string CitizenshipNumber { get; set; }
        public string Password { get; set; }
        public string BarberShopName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string District { get; set; }
        public string Type { get; set; }
        public DateTime RequestDate { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
