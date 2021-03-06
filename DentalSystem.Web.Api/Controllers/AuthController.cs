using System;
using System.Threading.Tasks;
using DentalSystem.UseCases.Identity.Dto.Input;
using DentalSystem.Interfaces.UseCases.Identity.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalSystem.Web.Api.Controllers
{
    [ApiController]
    [Route ("api/[controller]")]
    [Authorize (AuthenticationSchemes = "Bearer")]
    public class AuthController : BaseApiController
    {
        public Lazy<ILoginCommand> LoginCommand { get; }

        public Lazy<IRegisterUserCommand> RegisterUserCommand { get; }

        public AuthController(
            Lazy<ILoginCommand> loginCommand,
            Lazy<IRegisterUserCommand> registerUserCommand)
        {
            RegisterUserCommand = registerUserCommand;
            LoginCommand = loginCommand;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route ("register")]
        public async Task<IActionResult> Register(RegisterUserInput model)
        {
            var result =  await RegisterUserCommand.Value.RegisterAsync(model);
            return PresentResult(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route ("login")]
        public async Task<IActionResult> Login([FromBody] UserCredentialsInput loginCredentials)
        {
            var result =  await LoginCommand.Value.LoginAsync(loginCredentials);
            return PresentResult(result);
        }
    }
}