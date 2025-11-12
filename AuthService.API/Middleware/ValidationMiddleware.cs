using FluentValidation;
using HotChocolate.Resolvers;

namespace AuthService.API.Middlewares
{
    public class ValidationMiddleware
    {
        private readonly FieldDelegate _next;

        public ValidationMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var input = context.ArgumentValue<object?>("input");
            if (input != null)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(input.GetType());
                var validator = context.Services.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(input));

                    if (!validationResult.IsValid)
                    {
                        context.ReportError($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}

