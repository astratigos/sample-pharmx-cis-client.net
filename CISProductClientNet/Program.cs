using CISProductClientNet.Behaviours;
using CISProductClientNet.Processors;
using DataSync.Shared.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logger =>
    {
        logger.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        var Configuration = context.Configuration;
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        services.AddSingleton<IDataSyncClient>(x => new DataSyncClient(
            siteID: Configuration["DataSync:SiteID"],
            serviceID: Configuration["DataSync:ServiceID"],
            inboundKey: new DataSyncClientKey()
            {
                SharedAccessKeyName = $"{Configuration["DataSync:SiteID"]}.{Configuration["DataSync:ServiceID"]}",
                SharedAccessKey = Configuration["DataSync:InboundSharedAccessKey"],
            },
            outboundKey: new DataSyncClientKey()
            {
                SharedAccessKeyName = $"{Configuration["DataSync:SiteID"]}.{Configuration["DataSync:ServiceID"]}",
                SharedAccessKey = Configuration["DataSync:OutboundSharedAccessKey"],
            },
            options: new DataSyncClientOptions()
            {
                transportType = "Amqp",
                maxConcurrentSessions = 1,
                maxConcurrentCallsPerSession = 1,
                maxConcurrentCalls = 1,
                autoCompleteMessages = false,
                prefetchCount = 100
            },
            sessionEnabled: false,
            environment: Configuration["DataSync:Environment"]
        ));
        services.AddHostedService<MessageProcessor>();
    })
    .Build();

await host.RunAsync();