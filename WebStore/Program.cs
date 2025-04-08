using Microsoft.EntityFrameworkCore;
using WebStore.Assignments;
using WebStore.Entities;

namespace WebStore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new WebStoreContext();

            var assignments = new LinqQueriesAssignment(context);

            await assignments.Task01_ListAllCustomers();
            await assignments.Task02_ListOrdersWithItemCount();
            await assignments.Task03_ListProductsByDescendingPrice();
            await assignments.Task04_ListPendingOrdersWithTotalPrice();
            await assignments.Task05_OrderCountPerCustomer();
            await assignments.Task06_Top3CustomersByOrderValue();
            await assignments.Task07_RecentOrders();
            await assignments.Task08_TotalSoldPerProduct();
            await assignments.Task09_DiscountedOrders();
            await assignments.Task10_AdvancedQueryExample();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
