using AutoFixture;
using EventSourcingImplementation;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EventSourcingImplementation.Events;

namespace EventSourcing.Test
{
    public class WarehouseProductTest
    {
        private readonly string sku;

        private readonly int initialQuantity;

        private readonly WarehouseProduct warehouseProduct;
       
        private readonly Fixture fixture;

        public WarehouseProductTest()
        {
            fixture = new Fixture();
            fixture.Customizations.Add(new Int32SequenceGenerator());
            sku = fixture.Create<string>();
            initialQuantity = (int)fixture.Create<uint>();

            warehouseProduct = WarehouseProduct.Load(sku, new[]
            {
                new ProductReceived(sku, initialQuantity, DateTime.UtcNow)

            });
        }

        [Fact]
        public void ShipProductShouldRaiseProductShippted()
        {
            var quantityToShip = fixture.Create<int>();
            warehouseProduct.ShipProduct(quantityToShip);

            var outEvents = warehouseProduct.GetUncommittedEvents();
            outEvents.Count.ShouldBe(1);

            var outEvent = outEvents.Single();
            outEvent.ShouldBeOfType<ProductShipped>();

            var productShipped = (ProductShipped)outEvent;
            productShipped.ShouldSatisfyAllConditions(
                x => x.Quantity.ShouldBe(quantityToShip),
                x => x.Sku.ShouldBe(sku),
                x=> x.EventType.ShouldBe("ProductShipped")
                );
        }

        [Fact]
        public void ShipProductShouldThrowExceptionIfNoQuantityOnHand()
        {
            var exception = Should.Throw<InvalidDomainException>(() => warehouseProduct.ShipProduct(initialQuantity + 1));
            exception.Message.ShouldBe("Cannot Ship to a negative Quantity on Hand.");
        }

        [Fact]
        public void ReceiveProductShouldRaiseProductReceived()
        {
            var quantityReceived = fixture.Create<int>();
            warehouseProduct.ReceiveProduct(quantityReceived);

            var outEvents = warehouseProduct.GetUncommittedEvents();
            outEvents.Count.ShouldBe(1);
            var outEvent = outEvents.Single();
            outEvent.ShouldBeOfType<ProductReceived>();

            var productReceived = (ProductReceived)outEvent;
            productReceived.ShouldSatisfyAllConditions(
                x => x.Quantity.ShouldBe(quantityReceived),
                x => x.Sku.ShouldBe(sku),
                x => x.EventType.ShouldBe("ProductReceived")
                );
        }

        [Fact]
        public void AdjustInventoryShouldRaiseProductAdjusted()
        {
            var quantityAdjusted = fixture.Create<int>();
            var reason = fixture.Create<string>();
            warehouseProduct.AdjustInventory(quantityAdjusted, reason);

            var outEvents = warehouseProduct.GetUncommittedEvents();
            outEvents.Count.ShouldBe(1);
            var outEvent = outEvents.Single();
            outEvent.ShouldBeOfType<InventoryAdjusted>();

            var adjustedInventory = (InventoryAdjusted)outEvent;
            adjustedInventory.ShouldSatisfyAllConditions(
                x => x.Quantity.ShouldBe(quantityAdjusted),
                x => x.Sku.ShouldBe(sku),
                x =>x.Reason.ShouldBe(reason),
                x => x.EventType.ShouldBe("InventoryAdjusted")
                );
        }

        [Fact]
        public void AdjustedInventoryShouldThrowExceptionIfNoQuantityOnHand()
        {
            var exception = Should.Throw<InvalidDomainException>(()=> warehouseProduct.AdjustInventory((initialQuantity + 1) * -1, string.Empty));
            exception.Message.ShouldBe("Cannot adjust to a negative Quantity on Hand.");
        }
    }
}
