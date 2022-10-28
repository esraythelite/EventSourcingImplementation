using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingImplementation
{
    public class Events
    {
        public record ProductShipped(string Sku, int Quantity, DateTime DateTime) : IEvent
        {
            public string EventType { get; } = "ProductShipped";
        }

        public record ProductReceived(string Sku, int Quantity, DateTime DateTime) : IEvent
        {
            public string EventType { get; } = "ProductReceived";
        }


        public record InventoryAdjusted(string Sku, int Quantity, string Reason, DateTime DateTime) : IEvent
        {
            public string EventType { get; } = "InventoryAdjusted";
        }

    }
}
