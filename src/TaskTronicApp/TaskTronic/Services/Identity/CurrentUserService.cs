namespace TaskTronic.Services.Identity
{
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using System.Security.Claims;

    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal user;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.user = httpContextAccessor.HttpContext?.User;

            if (user is null)
            {
                throw new System.InvalidOperationException("The request does not contain an authenticated user.");
            }

            this.UserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string UserId { get; }

        public bool IsAdministrator => this.user.IsAdministrator();
    }
}
