﻿using DataSync.Shared;
using DataSync.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace CISProductClientNet.Commands
{
    // This attribute filters messages based on their type
    [ServiceBusMessageFilters(MessageType.ItemMaintenance)]
    public class ItemReceived : DataSyncMessageReceievedEventHandler
    {
        ILogger<ItemReceived> logger { get; set; }
        public ItemReceived(ILogger<ItemReceived> _logger)
        {
            logger = _logger;
        }

        public async override Task HandleServiceBusEvent(DataSyncMessage inboundMessage, CancellationToken cancellationToken)
        {

            logger.LogInformation("Handling Item");
            // Do Work Here
            // If Task is completed succesfully. Message will be marked as completed.
            // If work is not succesful then MessageProcessor will have an error event raised.
        }
    }
}
