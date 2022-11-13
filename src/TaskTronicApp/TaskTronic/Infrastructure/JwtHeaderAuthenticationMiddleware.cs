namespace TaskTronic.Infrastructure
{
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System.Threading.Tasks;
    using TaskTronic.Services.Identity;

    public class JwtHeaderAuthenticationMiddleware : IMiddleware
    {
        private readonly ICurrentTokenService currentToken;

        public JwtHeaderAuthenticationMiddleware(ICurrentTokenService currentToken)
            => this.currentToken = currentToken;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Headers[InfrastructureConstants.AuthorizationHeaderName].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                this.currentToken.Set(token.Split().Last());
            }

            await next.Invoke(context);
        }
    }
}
