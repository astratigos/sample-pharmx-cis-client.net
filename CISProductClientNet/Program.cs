using CISProductClientNet.Processors;
using DataSync.Shared.SDK;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logger =>
    {
        logger.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        var Configuration = context.Configuration;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton<IDataSyncClient>(x => new DataSyncClient(
            siteID: Configuration["DataSync:SiteID"],
            serviceID: Configuration["DataSync:ServiceID"],
            inboundKey: new DataSyncClientKey()
            {
                SharedAccessKeyName = $"{Configuration["DataSync:SiteID"]}",
                SharedAccessKey = Configuration["DataSync:InboundSharedAccessKey"],
            },
            outboundKey: new DataSyncClientKey()
            {
                SharedAccessKeyName = $"{Configuration["DataSync:SiteID"]}",
                SharedAccessKey = Configuration["DataSync:OutboundSharedAccessKey"],
            },
            options: new DataSyncClientOptions()
            {
                transportType = "Amqp",
                maxConcurrentCalls = Configuration.GetValue<int>("DataSync:Options:MaxConcurrentCalls"),
                prefetchCount = Configuration.GetValue<int>("DataSync:Options:PrefetchCount"),
                autoCompleteMessages = false
            },
            sessionEnabled: false,
            environment: Configuration["DataSync:Environment"]
        ));
        services.AddHostedService<MessageProcessor>();
    })
    .Build();

await host.RunAsync();