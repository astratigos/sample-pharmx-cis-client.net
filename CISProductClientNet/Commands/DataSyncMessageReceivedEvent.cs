using DataSync.Shared;
using DataSync.Shared.Enums;
using MediatR;

namespace CISProductClientNet.Commands
{
    public record DataSyncMessageReceivedEvent(DataSyncMessage message) : INotification;

    public abstract class DataSyncMessageReceievedEventHandler : INotificationHandler<DataSyncMessageReceivedEvent>
    {
        public abstract Task HandleServiceBusEvent(DataSyncMessage inboundMessage, CancellationToken cancellationToken);

        public Task Handle(DataSyncMessageReceivedEvent request, CancellationToken cancellationToken)
        {
            if (MatchesFilter(request.message.MessageType))
            {
                return HandleServiceBusEvent(request.message, cancellationToken);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        private bool MatchesFilter(MessageType messageType)
        {
            List<ServiceBusMessageFilters> InheritedAttributes = ((ServiceBusMessageFilters[])System.Attribute.GetCustomAttributes(this.GetType(), typeof(ServiceBusMessageFilters), true)).ToList();

            if (InheritedAttributes.Any(attribute => messageType == attribute.MessageType))
            {
                return true;
            }

            return false;
        }
    }

    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Method), Inherited = true, AllowMultiple = true)]
    public sealed class ServiceBusMessageFilters : Attribute
    {
        public ServiceBusMessageFilters(MessageType messageType)
        {
            MessageType = messageType;
        }

        public MessageType MessageType { get; set; }
    }
}
