using AuthService.Infrastructure.Entities;

namespace AuthService.API.GraphQL.Types
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Field(u => u.PasswordHash).Ignore();

            descriptor.Field("email")
                .Resolve(context =>
                {
                    var httpContext = context.Service<IHttpContextAccessor>()?.HttpContext;
                    if (httpContext?.User.Identity?.IsAuthenticated ?? false)
                    {
                        return context.Parent<User>().Email;
                    }
                    return "You must authenticate to view!";
                })
                .Type<StringType>();

            descriptor.Field(u => u.Id).Type<NonNullType<UuidType>>();
        }
    }
}
