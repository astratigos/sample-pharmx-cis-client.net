using Azure.Messaging.ServiceBus;
using DataSync.Shared.SDK;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CISProductClientNet.Processors
{
    public class MessageProcessor : BackgroundService
    {
        private readonly ILogger<MessageProcessor> _logger;
        private readonly IDataSyncClient _dataSyncClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageProcessor(
            ILogger<MessageProcessor> logger,
            IDataSyncClient dataSyncClient,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _logger = logger;
            _dataSyncClient = dataSyncClient;
            _serviceScopeFactory = serviceScopeFactory;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Message Processor running at: {time}", DateTimeOffset.Now);

            try
            {
                _dataSyncClient.ProcessDataSyncMessageAsync += MessageHandler;
                _dataSyncClient.ProcessDataSyncErrorAsync += ErrorHandler;

                await _dataSyncClient.StartProcessingAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        private async Task MessageHandler(DataSyncMessageReceivedEvent eventArgs)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                Stopwatch stopWatch = Stopwatch.StartNew();
                var mediatR = scope.ServiceProvider.GetService<IMediator>();

                try
                {
                    await mediatR.Publish(new Commands.DataSyncMessageReceivedEvent(eventArgs.DataSyncMessage));
                    await eventArgs.CompleteAsync();
                    _logger.LogInformation($"[SUCCESS] Message Processed: {eventArgs.DataSyncMessage.MessageId}. Type: {eventArgs.DataSyncMessage.MessageType}. ms: {stopWatch.Elapsed.TotalMilliseconds}");
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"[FAILED] Message Processed: {eventArgs.DataSyncMessage.MessageId}. Type: {eventArgs.DataSyncMessage.MessageType}. ms: {stopWatch.Elapsed.TotalMilliseconds}. Error: {e.Message}");
                    throw;
                }
                finally
                {
                    stopWatch.Stop();
                }
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // TODO: Handle any other error Here.
            return Task.CompletedTask;
        }
    }
}
