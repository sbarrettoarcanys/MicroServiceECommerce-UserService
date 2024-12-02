using ECommerce.UserService.BusinessLogic.IManagers;
using ECommerce.UserService.DataAccess.Data;
using ECommerce.UserService.DataAccess.Repository;
using ECommerce.UserService.DataAccess.Repository.IRepository;
using ECommerce.UserService.Models.Models;
using ECommerce.UserService.Models.ViewModels;
using ECommerce.UserService.Utility;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.UserService.BusinessLogic.Managers
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBusClient _messageBusClient;

        public AuthService(IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork, IMessageBusClient? messageBusClient)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _messageBusClient = messageBusClient;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            ApplicationUser user = _unitOfWork._applicationUserRepository.Get(x => x.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;

        }

        public async Task<LoginResponseViewModel> Login(LoginRequestViewModel loginRequestViewModel)
        {
            var user = _unitOfWork._applicationUserRepository.Get(x => x.UserName.ToLower() == loginRequestViewModel.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestViewModel.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseViewModel() { User = null, Token = "" };
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            ////set jwt token
            //_tokenProvider.SetToken(token);

            UserViewModel userViewModel = new UserViewModel()
            {
                Email = user.Email,
                ExternalId = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                StreetAddress = user.StreetAddress,

            };
            LoginResponseViewModel LoginRequestViewModel = new LoginResponseViewModel()
            {
                User = userViewModel,
                Token = token
            };

            return LoginRequestViewModel;
        }

        public async Task<RegistrationRequestViewModel> Register(RegistrationRequestViewModel registrationRequestViewModel)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registrationRequestViewModel.Email,
                Email = registrationRequestViewModel.Email,
                NormalizedEmail = registrationRequestViewModel.Email.ToUpper(),
                Name = registrationRequestViewModel.Name,
                PhoneNumber = registrationRequestViewModel.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registrationRequestViewModel.Password);
            if (result.Succeeded)
            {

                //bool isRoleAssigned = await AssignRole(registrationRequestViewModel.Email, ConstantValues.Role_Customer);

                var isRoleAssigned = await _userManager.AddToRoleAsync(user, ConstantValues.Role_Customer);

                if (!isRoleAssigned.Succeeded)
                {
                    registrationRequestViewModel.ErrorMessage = "Error assigning role";
                }
                else
                {
                    registrationRequestViewModel.Role = ConstantValues.Role_Customer;
                }

                registrationRequestViewModel.Id = user.Id;
                _messageBusClient.PublishNewUser(registrationRequestViewModel);
            }
            else
            {
                registrationRequestViewModel.Id = string.Empty;
                registrationRequestViewModel.ErrorMessage = result.Errors.FirstOrDefault().Description;
            }
            return registrationRequestViewModel;
        }
    }
}
