using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingImplementation
{
    public class WareHouseProductRepository
    {
        private readonly Dictionary<string, List<IEvent>> inMemoryStreams = new();

        public WarehouseProduct Get(string sku)
        {
            var warehouseProduct = new WarehouseProduct(sku);

            if (inMemoryStreams.ContainsKey(sku))
            {
                foreach (IEvent evnt in inMemoryStreams[sku])
                {
                    warehouseProduct.AddEvent(evnt);
                }
            }
            return warehouseProduct;
        }

        public void Save(WarehouseProduct warehouseProduct)
        {
            if (inMemoryStreams.ContainsKey(warehouseProduct.Sku) == false)
            {
                inMemoryStreams.Add(warehouseProduct.Sku, new List<IEvent>());
            }

            var newEvents = warehouseProduct.GetUncommittedEvents();

            inMemoryStreams[warehouseProduct.Sku].AddRange(newEvents);
            warehouseProduct.ClearUncommittedEvents();
        }
    }
}
