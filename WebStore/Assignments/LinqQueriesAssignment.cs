using Microsoft.EntityFrameworkCore;
using WebStore.Entities;

namespace WebStore.Assignments
{
    public class LinqQueriesAssignment
    {
        private readonly WebStoreContext _dbContext;

        public LinqQueriesAssignment(WebStoreContext context)
        {
            _dbContext = context;
        }

        public async Task Task01_ListAllCustomers()
        {
            var customers = await _dbContext.Customers.ToListAsync();

            Console.WriteLine("=== TASK 01: List All Customers ===");
            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName} - {c.Email}");
            }
        }

        public async Task Task02_ListOrdersWithItemCount()
        {
            var orders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ToListAsync();

            Console.WriteLine("=== TASK 02: List Orders With Item Count ===");
            foreach (var order in orders)
            {
                var totalItems = order.OrderItems.Sum(oi => oi.Quantity);
                Console.WriteLine($"Order #{order.OrderId} | Customer: {order.Customer.FirstName} {order.Customer.LastName} | Status: {order.OrderStatus} | Total Items: {totalItems}");
            }
        }

        public async Task Task03_ListProductsByDescendingPrice()
        {
            var products = await _dbContext.Products
                .OrderByDescending(p => p.Price)
                .ToListAsync();

            Console.WriteLine("=== Task 03: List Products By Descending Price ===");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.ProductName} - ${p.Price:F2}");
            }
        }

        public async Task Task04_ListPendingOrdersWithTotalPrice()
        {
            var pendingOrders = await _dbContext.Orders
                .Where(o => o.OrderStatus == "Pending")
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ToListAsync();

            Console.WriteLine("=== Task 04: List Pending Orders With Total Price ===");
            foreach (var order in pendingOrders)
            {
                var total = order.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount);
                Console.WriteLine($"Order #{order.OrderId} | Customer: {order.Customer.FirstName} {order.Customer.LastName} | Date: {order.OrderDate} | Total: ${total:F2}");
            }
        }

        public async Task Task05_OrderCountPerCustomer()
        {
            var orderCounts = await _dbContext.Orders
                .GroupBy(o => o.Customer)
                .Select(g => new
                {
                    CustomerName = g.Key.FirstName + " " + g.Key.LastName,
                    OrderCount = g.Count()
                })
                .ToListAsync();

            Console.WriteLine("=== Task 05: Order Count Per Customer ===");
            foreach (var item in orderCounts)
            {
                Console.WriteLine($"{item.CustomerName} - Orders: {item.OrderCount}");
            }
        }

        public async Task Task06_Top3CustomersByOrderValue()
        {
            var topCustomers = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .GroupBy(o => o.Customer)
                .Select(g => new
                {
                    Customer = g.Key,
                    TotalValue = g.SelectMany(o => o.OrderItems)
                                  .Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount)
                })
                .OrderByDescending(x => x.TotalValue)
                .Take(3)
                .ToListAsync();

            Console.WriteLine("=== Task 06: Top 3 Customers By Order Value ===");
            foreach (var c in topCustomers)
            {
                Console.WriteLine($"{c.Customer.FirstName} {c.Customer.LastName} - Total Order Value: ${c.TotalValue:F2}");
            }
        }

        public async Task Task07_RecentOrders()
        {
            var dateLimit = DateTime.Now.AddDays(-30);
            var recentOrders = await _dbContext.Orders
                .Where(o => o.OrderDate >= dateLimit)
                .Include(o => o.Customer)
                .ToListAsync();

            Console.WriteLine("=== Task 07: Recent Orders ===");
            foreach (var order in recentOrders)
            {
                Console.WriteLine($"Order #{order.OrderId} | Date: {order.OrderDate} | Customer: {order.Customer.FirstName} {order.Customer.LastName}");
            }
        }

        public async Task Task08_TotalSoldPerProduct()
        {
            var productSales = await _dbContext.OrderItems
                .GroupBy(oi => oi.Product)
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .ToListAsync();

            Console.WriteLine("=== Task 08: Total Sold Per Product ===");
            foreach (var item in productSales)
            {
                Console.WriteLine($"{item.ProductName} - Sold: {item.TotalSold}");
            }
        }

        public async Task Task09_DiscountedOrders()
        {
            var discountedOrders = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => oi.Discount > 0))
                .ToListAsync();

            Console.WriteLine("=== Task 09: Discounted Orders ===");
            foreach (var order in discountedOrders)
            {
                var discountedItems = order.OrderItems
                    .Where(oi => oi.Discount > 0)
                    .Select(oi => oi.Product.ProductName)
                    .ToList();

                Console.WriteLine($"Order #{order.OrderId} | Customer: {order.Customer.FirstName} {order.Customer.LastName} | Discounted Products: {string.Join(", ", discountedItems)}");
            }
        }

        public async Task Task10_AdvancedQueryExample()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 10: Advanced Query Example ===");

            var result = await _dbContext.Products
                .Where(p => p.Categories.Any(c => c.CategoryName == "Electronics"))
                .Select(p => new
                {
                    ProductName = p.ProductName,
                    Orders = p.OrderItems
                        .Select(oi => oi.Order)
                        .Where(o => o.OrderItems.Any(oi => oi.ProductId == p.ProductId))
                        .ToList(),
                    TopStore = p.Stocks
                        .OrderByDescending(s => s.QuantityInStock)
                        .Select(s => s.Store.StoreName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            foreach (var item in result)
            {
                Console.WriteLine($"Product: {item.ProductName}");
                Console.WriteLine($"Top Store: {item.TopStore}");
                Console.WriteLine("Orders:");
                foreach (var order in item.Orders)
                {
                    Console.WriteLine($"  Order ID: {order.OrderId}, Date: {order.OrderDate}");
                }
                Console.WriteLine();
            }
        }
    }
}
