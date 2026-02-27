using System.Collections.Generic;
using System.Linq;

namespace SieMarket
{

    public class Customer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public List<Order> allOrders = new List<Order>();

    }

   public class Item
    {
        
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public Item(string _productName, decimal _price)
        {
                ProductName = _productName;
                Price = _price;
        }

        public override bool Equals(object obj)
        {
            if (obj is Item other)
            {
                return ProductName == other.ProductName && Price == other.Price;
            }
            return false;
        }

        //I'm overriding equals and gethashcode because in the order class I chose to go with a dictionary
        //so that means that if I don't override then I might end up with 2 of the same items treated differently
        //since dict normally checks for reference 
        public override int GetHashCode() => (Price, ProductName).GetHashCode();

    }


    public class Order
    {
        //Thing is that in this dictioary we should really add copies of the item because
        //if the price changes and we have the reference to the original item then we'd get 
        //price changes here too.

        //Another way I think this could be done is we'd just separate the Item and the Item from the order
        //like have two classes, one that points to the real item in the catalog and one that points to items we create 
        //specifically for the order.
        public IDictionary<Item, int> Items { get; set; }

        //2.2
        public decimal TotalPrice()
        {
            decimal total = Items.Sum(item => item.Key.Price * item.Value);
            return total > 500 ? total * 0.9m : total;
        }

    }


    public static class Shop
    {
        //2.3
        public static string GetCustomerWhoSpentMost(List<Customer> allCustomers)
        {
            if (allCustomers == null || !allCustomers.Any())
                return "There are are no customers";

            var topSpender = allCustomers
            .Select(
                customer => new
            {
                CustomerName = customer.Name,
                TotalSpent = customer.allOrders.Sum(o => o.TotalPrice())
            })
            .OrderByDescending(c => c.TotalSpent)
            .FirstOrDefault();

            return topSpender.CustomerName;
        }

        //2.4(Bonus)
        public static Dictionary<string, int> GetPopularProducts(List<Customer> allCustomers)
        {

            //I'm not sure about what the popularity of an item entails. Maybe it's like top 5 products sold or top 10 or w/e.
            //I decided to return a dictionary that was orderered beforehand in descending order so the top products are at the top and 
            //if someone needs the top 5 products then they can just get them or if it's top 10 or top 1 or top 3.
            return allCustomers
                .SelectMany(customers => customers.allOrders)
                .SelectMany(orders => orders.Items)
                .GroupBy(item => item.Key)
                .Select(group => new
                {
                    ProductName = group.Key.ProductName,
                    TotalAmountSold = group.Sum(item => item.Value)
                })
                .OrderByDescending (product => product.TotalAmountSold)
                .ToDictionary(product => product.ProductName, product => product.TotalAmountSold);
        }
    }
}
