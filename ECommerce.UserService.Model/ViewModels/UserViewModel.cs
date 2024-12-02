namespace ECommerce.UserService.Models.ViewModels
{
    public class UserViewModel
    {
        public string ExternalId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
    }

}
