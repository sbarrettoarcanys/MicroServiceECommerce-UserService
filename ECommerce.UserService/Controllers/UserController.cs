using Azure;
using ECommerce.UserService.BusinessLogic.IManagers;
using ECommerce.UserService.Models.ViewModels;
using ECommerce.UserService.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace ECommerce.UserService.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService; 
        protected ResponseViewModel _response;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserController(IAuthService authService, IHttpContextAccessor? contextAccessor)
        {
            _authService = authService;
            _contextAccessor = contextAccessor;
            _response = new();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestViewModel model)
        {

            try
            {
                var result = await _authService.Register(model);
                _response.Result = result;

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    _response.IsSuccess = false;
                    _response.Message = result.ErrorMessage;
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result = ex.Message;
                return BadRequest(_response);
            }

            //await _messageBus.PublishMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestViewModel model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }


            _contextAccessor.HttpContext?.Response.Cookies.Append(ConstantValues.TokenCookie, loginResponse.Token);

            _response.Result = loginResponse;

            return Ok(_response);

        }

        //[HttpPost("AssignRole")]
        //[Authorize(Roles = ConstantValues.Role_Admin)]
        //public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestViewModel model)
        //{
        //    var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role);
        //    if (!assignRoleSuccessful)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = "Error encountered";
        //        return BadRequest(_response);
        //    }
        //    return Ok(_response);

        //}
    }
}
