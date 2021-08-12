namespace LoopUp.Siesta.ServiceCollectionExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http.Headers;
    using LoopUp.Siesta.HttpDelegatingHandlers;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> that can be used to add a Siesta client.
    /// </summary>
    public static class AddSiestaClientExtensions
    {
        /// <summary>
        /// Add a Siesta Client of type T. This will include the use of Serilog logging and a correlationId.
        /// </summary>
        /// <param name="services">The IServiceCollection this.</param>
        /// <param name="baseAddress">Base address Uri for the Siesta client.</param>
        /// <param name="systemName">The name of the calling system.</param>
        /// <param name="defaultHeaders">Any default headers you which to apply to the client.</param>
        /// <param name="authenticationHeaderValue">Optional value for the authentication header.</param>
        /// <param name="loggerCorrelationIdKey">Optional key for the logging correlation Id.</param>
        /// <param name="requestHeaderCorrelationIdKey">Optional key for where to find the correlation id in the request.</param>
        /// <typeparam name="T">The type of Siesta Client to add.</typeparam>
        /// <returns>The IServiceCollection.</returns>
        public static IServiceCollection AddSiestaClientWithCorrelationIdAndSerilog<T>(
            this IServiceCollection services,
            Uri baseAddress,
            string systemName,
            Dictionary<string, string>? defaultHeaders = null,
            AuthenticationHeaderValue? authenticationHeaderValue = null,
            string? loggerCorrelationIdKey = null,
            string? requestHeaderCorrelationIdKey = null)
            where T : SiestaClient
        {
            var siestaClientConfigurationOptions = new SiestaClientConfigurationOptions
            {
                RequestHeaderCorrelationIdKey = requestHeaderCorrelationIdKey ?? "X-Correlation-ID",
            };
            services.AddSingleton(siestaClientConfigurationOptions);
            services.AddHttpClient();
            services.AddTransient(_ =>
                new CorrelationIdHandler(requestHeaderCorrelationIdKey ?? "X-Correlation-ID"));
            services.AddTransient(provider =>
                new SerilogHandler(
                    provider.GetRequiredService<ILogger>(),
                    systemName,
                    loggerCorrelationIdKey ?? "CorrelationId",
                    requestHeaderCorrelationIdKey ?? "X-Correlation-ID"));

            services.AddHttpClient<T>(client =>
                    {
                        client.BaseAddress = baseAddress;
                        foreach (var header in defaultHeaders ?? new Dictionary<string, string>())
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }

                        if (authenticationHeaderValue is not null)
                        {
                            client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                        }
                    })
                .AddHttpMessageHandler<CorrelationIdHandler>()
                .AddHttpMessageHandler<SerilogHandler>();

            return services;
        }
    }
}
