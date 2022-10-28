using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EventSourcingImplementation.Events;

namespace EventSourcingImplementation
{
    public class WarehouseProduct:AggregateRoot
    {
        
        public string Sku { get; }

        private readonly WarehouseProductState warehouseProductState = new();

        private readonly IList<IEvent> events = new List<IEvent>();

        public WarehouseProduct(string sku)
        {
            Sku = sku;
        }

        public override void Load(IEnumerable<IEvent> events)
        {
            foreach (var evnt in events)
            {
                Apply(evnt as dynamic);
            }
        }

        public static WarehouseProduct Load(string sku, IEnumerable<IEvent> eventList)
        {
            var warehouseProduct = new WarehouseProduct(sku);
            warehouseProduct.Load(eventList);
            return warehouseProduct;
        }

        public WarehouseProductState GetState()
        {
            return warehouseProductState;
        }

        public void ShipProduct(int quantity)
        {
            if (quantity > warehouseProductState.QuantityOnHand)
            {
                throw new InvalidDomainException("Cannot Ship to a negative Quantity on Hand.");
            }

            var productShipped = new ProductShipped(Sku, quantity, DateTime.UtcNow);
            AddEvent(productShipped);
            Add(productShipped);
        }

        private void Apply(ProductShipped evnt)
        {
            warehouseProductState.QuantityOnHand -= evnt.Quantity;
        }

        public void ReceiveProduct(int quantity)
        {
            var productReceived = new ProductReceived(Sku, quantity, DateTime.UtcNow);
            AddEvent(productReceived);
            Add(productReceived);
        }
        private void Apply(ProductReceived evnt) 
        {
            warehouseProductState.QuantityOnHand += evnt.Quantity;
        }

        public void AdjustInventory(int quantity, string reason)
        {
            if (warehouseProductState.QuantityOnHand + quantity <0)
            {
                throw new InvalidDomainException("Cannot adjust to a negative Quantity on Hand.");
            }

            var inventoryAdjusted = new InventoryAdjusted(Sku, quantity, reason, DateTime.UtcNow);
            AddEvent(inventoryAdjusted);
            Add(inventoryAdjusted);
        }


        private void Apply(InventoryAdjusted evnt) 
        {
            warehouseProductState.QuantityOnHand += evnt.Quantity;
        }

        public void AddEvent(IEvent evnt)
        {
            switch (evnt)
            {
                case ProductShipped shipProduct:
                    Apply(shipProduct);
                    break;
                case ProductReceived receiveProduct:
                    Apply(receiveProduct);
                    break;
                case InventoryAdjusted inventoryAdjusted:
                    Apply(inventoryAdjusted);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Event.");
            }

            events.Add(evnt);
        }

        public IList<IEvent> GetEvents()
        {
            return events;
        }

        public int GetQuantityOnHand()
        {
            return warehouseProductState.QuantityOnHand;
        }

    }


}
