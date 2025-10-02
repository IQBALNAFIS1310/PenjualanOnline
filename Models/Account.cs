namespace PenjualanOnline.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = "user";
    }
}
