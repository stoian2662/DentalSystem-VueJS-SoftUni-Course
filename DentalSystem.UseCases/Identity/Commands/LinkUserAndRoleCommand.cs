using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSystem.UseCases.Common.Dto.Output;
using DentalSystem.Entities.Identity;
using DentalSystem.Interfaces.Infrastructure.Identity;
using DentalSystem.Interfaces.UseCases.Identity.Dto.Input;
using DentalSystem.Interfaces.UseCases.Common.Dto.Output;
using DentalSystem.Interfaces.UseCases.Identity.Commands;
using DentalSystem.Interfaces.UseCases.Common.Validation;
using DentalSystem.UseCases.Common.Validation;
using Microsoft.AspNetCore.Identity;

namespace DentalSystem.UseCases.Identity.Commands
{
    public class LinkUserAndRoleCommand : ILinkUserAndRoleCommand
    {
        public IUserService<User> UserService { get; }

        public IRoleService<IdentityRole> RoleService { get; }

        public IApplicationValidator<ILinkUserAndRoleInput> Validator { get; }

        public LinkUserAndRoleCommand(
            IUserService<User> userService,
            IRoleService<IdentityRole> roleService,
            IApplicationValidator<ILinkUserAndRoleInput> validator)
        {
            UserService = userService;
            RoleService = roleService;
            Validator = validator;
        }

        public async Task<IResult<IMessageOutput>> ExecuteAsync(ILinkUserAndRoleInput inputModel)
        {
            var validationResult = Validator.Validate(inputModel);
            if (validationResult.Errors.Count > 0)
            {
                return new Result<IMessageOutput>(validationResult.Errors);
            }

            var role = await RoleService.FindByNameAsync(inputModel.RoleName);
            if (role == null)
            {
                var validationError = new ValidationError()
                {
                    PropertyName = nameof(inputModel.RoleName),
                    Errors = new List<string>()
                };

                validationError.Errors.Add("Role doesn't exist");

                validationResult.Errors.Add(validationError);
            }

            var user = await UserService.FindByNameAsync(inputModel.UserName);
            if (user == null)
            {
                var validationError = new ValidationError()
                {
                    PropertyName = nameof(inputModel.UserName),
                    Errors = new List<string>()
                };

                validationError.Errors.Add("User doesn't exist");

                validationResult.Errors.Add(validationError);
            }

            if (validationResult.Errors.Count > 0)
            {
                return new Result<IMessageOutput>(validationResult.Errors);
            }

            await UserService.AddToRoleAsync(user, role.Name);

            return new Result<IMessageOutput>(
                new MessageOutput($"\"{inputModel.RoleName}\" role set to user \"{inputModel.UserName}\"")
            );
        }
    }
}