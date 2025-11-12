using AuthService.API.DTOs;
using FluentValidation;

namespace AuthService.API.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            When(x => !string.IsNullOrEmpty(x.Username), () =>
            {
                RuleFor(x => x.Username)
                    .MinimumLength(3).MaximumLength(20)
                    .WithMessage("Username must be between 3 and 20 characters.");
            });

            When(x => !string.IsNullOrEmpty(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Invalid email format.");
            });

            When(x => !string.IsNullOrEmpty(x.Password), () =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(8)
                    .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                    .Matches("[0-9]").WithMessage("Password must contain at least one number.");
            });
        }
    }
}
