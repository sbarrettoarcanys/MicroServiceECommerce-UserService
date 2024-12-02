namespace ECommerce.UserService.Models.ViewModels
{
    public class LoginResponseViewModel
    {
        public UserViewModel User { get; set; }
        public string Token { get; set; }
    }
}
