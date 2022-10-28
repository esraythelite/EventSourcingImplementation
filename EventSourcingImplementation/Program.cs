using EventSourcingImplementation;
using static EventSourcingImplementation.Events;

namespace Demo
{
    class Program
    {

        static void Main()
        {
            var warehouseProductRepository = new WareHouseProductRepository();
            var key = string.Empty;

            while (key != "X")
            {
                Console.WriteLine("R: Receive Inventory");
                Console.WriteLine("S: Ship Inventory");
                Console.WriteLine("A: Inventory Adjustment");
                Console.WriteLine("Q: Quantity On Hand");
                Console.WriteLine("E: Events");
                Console.WriteLine("> ");

                key = Console.ReadLine()?.ToUpperInvariant();
                Console.WriteLine();

                var sku = GetSkuFromConsole();
                var warehouseProduct = warehouseProductRepository.Get(sku);

                switch (key)
                {
                    case "R":
                        var receiveInput = GetQuantity();
                        if (receiveInput.IsValid)
                        {
                            warehouseProduct.ReceiveProduct(receiveInput.Quantity);
                            Console.WriteLine($"{sku} Received: {receiveInput.Quantity}");
                        }
                        break;
                    case "S":
                        var shipInput = GetQuantity();
                        if (shipInput.IsValid)
                        {
                            warehouseProduct.ShipProduct(shipInput.Quantity);
                            Console.WriteLine($"{sku} Shipped: {shipInput.Quantity}");
                        }
                        break;
                    case "A":
                        var adjustmentInput = GetQuantity();
                        if (adjustmentInput.IsValid)
                        {
                            var reason = GetAdjustmentReason();
                            warehouseProduct.AdjustInventory(adjustmentInput.Quantity, reason);
                            Console.WriteLine($"{sku} Adjusted: {adjustmentInput.Quantity} {reason}");
                        }
                        break;
                    case "Q":
                        var currentQuantityOnHand = warehouseProduct.GetQuantityOnHand();
                        Console.WriteLine($"{sku} Quantity On Hand: {currentQuantityOnHand}");
                        break;
                    case "E":
                        Console.WriteLine($"Events: {sku}");
                        foreach (var evnt in warehouseProduct.GetUncommittedEvents())
                        {
                            switch (evnt)
                            {
                                case ProductShipped shipProduct:
                                    Console.WriteLine($"{shipProduct.DateTime:u} {sku} Shipped:{shipProduct.Quantity}");
                                    break;
                                case ProductReceived receiveProduct:
                                    Console.WriteLine($"{receiveProduct.DateTime:u} {sku} Received:{receiveProduct.Quantity}");
                                    break;
                                case InventoryAdjusted inventoryAdjusted:
                                    Console.WriteLine($"{inventoryAdjusted.DateTime:u} {sku} Adjusted:{inventoryAdjusted.Quantity} {inventoryAdjusted.Reason} ");
                                    break;
                            }
                        }
                        break;
                }

                warehouseProductRepository.Save(warehouseProduct);

                Console.ReadLine();
                Console.WriteLine();
            }
        }
        private static string GetSkuFromConsole()
        {
            Console.Write("SKU: ");
            string? sku = Console.ReadLine();
            if (!string.IsNullOrEmpty(sku))
            {
                return sku;
            }
            return "Enter a sku";
        }

        private static string GetAdjustmentReason()
        {
            Console.Write("Reason: ");
            string? reason = Console.ReadLine();
            if (!string.IsNullOrEmpty(reason))
            {
                return reason;
            }
            return "Enter a adjustment reason";
        }

        private static (int Quantity, bool IsValid) GetQuantity()
        {
            Console.Write("Quantity: ");
            if (int.TryParse(Console.ReadLine(),out int quantity))
            {
                return (quantity, true);
            }
            else
            {
                Console.WriteLine("Invalid Quantity");
                return (0, false);
            }

        }

    }

}



