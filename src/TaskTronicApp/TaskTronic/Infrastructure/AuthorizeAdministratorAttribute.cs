namespace TaskTronic.Infrastructure
{
    using Microsoft.AspNetCore.Authorization;

    public class AuthorizeAdministratorAttribute : AuthorizeAttribute
    {
        public AuthorizeAdministratorAttribute()
            => this.Roles = GlobalConstants.AdministratorRoleName;
    }
}
