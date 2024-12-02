using ECommerce.UserService.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.UserService.BusinessLogic.IManagers
{
    public interface IAuthService
    {
        Task<RegistrationRequestViewModel> Register(RegistrationRequestViewModel registrationRequestDto);
        Task<LoginResponseViewModel> Login(LoginRequestViewModel loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}
