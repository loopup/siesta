namespace Siesta.Client.ServiceCollectionExtensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using Siesta.Client.HttpDelegatingHandlers;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> that can be used to add a Siesta client.
    /// </summary>
    public static class AddSiestaClientExtensions
    {
        /// <summary>
        /// Add a Siesta Client of type T. This will include the use of Serilog logging and a correlationId.
        /// </summary>
        /// <param name="services">The IServiceCollection this.</param>
        /// <param name="correlationAndLoggingConfigurationOptions">Configuration options for convenient client configuration.</param>
        /// <typeparam name="T">The type of Siesta Client to add.</typeparam>
        /// <returns>The IServiceCollection.</returns>
        public static IServiceCollection AddSiestaClient<T>(
            this IServiceCollection services,
            CorrelationAndLoggingConfigurationOptions correlationAndLoggingConfigurationOptions)
            where T : SiestaClient
        {
            var siestaClientConfigurationOptions = new SiestaClientConfigurationOptions
            {
                RequestHeaderCorrelationIdKey = correlationAndLoggingConfigurationOptions.RequestHeaderCorrelationIdKey,
            };
            services.AddSingleton(siestaClientConfigurationOptions);
            services.AddHttpClient();
            services.AddTransient(_ =>
                new CorrelationIdHandler(correlationAndLoggingConfigurationOptions.RequestHeaderCorrelationIdKey));
            services.AddTransient(provider =>
                new SerilogHandler(
                    provider.GetRequiredService<ILogger>(),
                    correlationAndLoggingConfigurationOptions.SystemName,
                    correlationAndLoggingConfigurationOptions.LoggerCorrelationId,
                    correlationAndLoggingConfigurationOptions.RequestHeaderCorrelationIdKey));

            services.AddHttpClient<T>(client =>
                    {
                        client.BaseAddress = correlationAndLoggingConfigurationOptions.BaseAddress;
                        foreach (var header in correlationAndLoggingConfigurationOptions.DefaultHeaders)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }

                        if (correlationAndLoggingConfigurationOptions.AuthenticationHeaderValue is not null)
                        {
                            client.DefaultRequestHeaders.Authorization = correlationAndLoggingConfigurationOptions.AuthenticationHeaderValue;
                        }
                    })
                .AddHttpMessageHandler<CorrelationIdHandler>()
                .AddHttpMessageHandler<SerilogHandler>();

            return services;
        }
    }
}
