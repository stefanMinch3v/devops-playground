namespace TaskTronic.Infrastructure
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;
    using System;
    using System.Net;
    using System.Net.Http.Headers;
    using TaskTronic.Services.Identity;

    public static class HttpClientBuilderExtensions
    {
        public static void WithConfiguration(
            this IHttpClientBuilder httpClientBuilder,
            string baseAddress)
            => httpClientBuilder
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.BaseAddress = new Uri(baseAddress);

                    var requestServices = serviceProvider
                        .GetService<IHttpContextAccessor>()
                        ?.HttpContext
                        .RequestServices;

                    var currentToken = requestServices
                        ?.GetService<ICurrentTokenService>()
                        ?.Get();

                    if (currentToken is null)
                    {
                        return;
                    }

                    var authorizationHeader = new AuthenticationHeaderValue(
                        InfrastructureConstants.AuthorizationHeaderValuePrefix, 
                        currentToken);

                    client.DefaultRequestHeaders.Authorization = authorizationHeader;
                })
                // try to connect max 6 times for a seconds range exponentially with math pow (2,4,6,8,16....)
                .AddTransientHttpErrorPolicy(policy => policy
                    .OrResult(result => result.StatusCode == HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(6, retry => TimeSpan.FromSeconds(Math.Pow(2, retry))))
                // if we have 5 failures in a row stop and wait for 30 seconds
                .AddTransientHttpErrorPolicy(policy => policy
                    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
    }
}
