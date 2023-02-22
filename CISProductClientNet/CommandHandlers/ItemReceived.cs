using CISProductClientNet.Commands;
using DataSync.Shared;
using DataSync.Shared.Enums;

namespace PharmXEDI.DataClient.ServiceWorker.Commands
{
    // This attribute filters messages based on their type
    [ServiceBusMessageFilters(MessageType.ItemMaintenance)]
    public class ItemReceived : DataSyncMessageReceievedEventHandler
    {

        public ItemReceived()
        {

        }

        public async override Task HandleServiceBusEvent(DataSyncMessage inboundMessage, CancellationToken cancellationToken)
        {
           // Do Work Here
           // If Task is completed succesfully. Message will be marked as completed.
           // If work is not succesful then MessageProcessor will have an error event raised.
        }
    }
}
