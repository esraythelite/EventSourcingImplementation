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
    public class WarehouseProductAggregateTest:AggreageTest<WarehouseProduct>
    {
        private readonly Fixture fixture;
        
        private readonly string sku = "init13";

        private readonly int initialQuantity;

        public WarehouseProductAggregateTest():base(new WarehouseProduct("init13"))
        {
            fixture = new Fixture();
            fixture.Customizations.Add(new Int32SequenceGenerator());
            initialQuantity = (int)fixture.Create<uint>();
        }

        [Fact]
        public void ShipProductShouldRaiseProductShipped()
        {
            Given(new ProductReceived(sku, initialQuantity, DateTime.UtcNow));

            var quantityToShip = fixture.Create<int>();
            When(x => x.ShipProduct(quantityToShip));

            Then<ProductShipped>(
                x => x.Quantity.ShouldBe(quantityToShip),
                x => x.Sku.ShouldBe(sku),
                x => x.EventType.ShouldBe("ProductShipped"));
        }

        [Fact]
        public void ShipProductShouldThrowExceptionIfNoQuantityOnHand()
        {
            Given();

            Throws<InvalidDomainException>(
                x => x.ShipProduct(1),
                x => x.Message.ShouldBe("Cannot Ship to a negative Quantity on Hand.")
                );
        }

        [Fact]
        public void ReceiveProductShouldRaiseProductReceived()
        {
            Given(new ProductReceived(sku, initialQuantity, DateTime.UtcNow));

            var quantityToReceive = fixture.Create<int>();
            When(x => x.ReceiveProduct(quantityToReceive));

            Then<ProductReceived>(
                x => x.Quantity.ShouldBe(quantityToReceive),
                X=> X.Sku.ShouldBe(sku),
                X=> X.EventType.ShouldBe("ProductReceived")
                );
        }

        [Fact]
        public void AdjustInventoryShouldRaiseProductAdjusted()
        {

            Given(new ProductReceived(sku, initialQuantity, DateTime.UtcNow));

            var quantityAdjusted = fixture.Create<int>();
            var reason = fixture.Create<string>();

            When(x=> x.AdjustInventory(quantityAdjusted, reason));

            Then<InventoryAdjusted>(
                x=>x.Quantity.ShouldBe(quantityAdjusted),
                Xunit=>Xunit.Sku.ShouldBe(sku),
                Xunit=>Xunit.Reason.ShouldBe(reason),
                Xunit=>Xunit.EventType.ShouldBe("InventoryAdjusted")
                );
        }

        [Fact]
        public void AdjustInventoryShouldThowExceptionIfNoQuantityOnHand()
        {
            Given();

            var reason = fixture.Create<string>();

            Throws<InvalidDomainException>(
                x => x.AdjustInventory(-1, reason),
                x => x.Message.ShouldBe("Cannot adjust to a negative Quantity on Hand.")
                );
        }
    }
}
