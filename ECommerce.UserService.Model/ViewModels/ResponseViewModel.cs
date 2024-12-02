namespace ECommerce.UserService.Models.ViewModels
{
    public class ResponseViewModel
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
