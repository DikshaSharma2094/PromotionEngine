using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine
{
    /// <summary>
    /// Initialize the product with their prices
    /// </summary>
    public class Product
    {
        public string Id { get; set; }
        public decimal Price { get; set; }
        public Product(string id)
        {
            this.Id = id;
            switch (id)
            {
                case "A":
                    this.Price = 50m;
                    break;
                case "B":
                    this.Price = 30m;
                    break;
                case "C":
                    this.Price = 20m;
                    break;
                case "D":
                    this.Price = 15m;
                    break;
            }
        }
    }
    /// <summary>
    /// List of Products based on orders placed
    /// </summary>
    public class Order
    {
        public int OrderID { get; set; }
        public List<Product> Products { get; set; }
        public Order(int _orderId, List<Product> _products)
        {
            this.OrderID = _orderId;
            this.Products = _products;
        }
    }
    /// <summary>
    /// List of all discounted prices to keep track based on product
    /// </summary>
    public class Discount
    {
        public int DiscountID { get; set; }
        public Dictionary<string, int> ProductInfo { get; set; }
        public decimal DiscountPrice { get; set; }

        public Discount(int _discountID, Dictionary<string, int> _prodInfo, decimal _discountPrice)
        {
            this.DiscountID = _discountID;
            this.ProductInfo = _prodInfo;
            this.DiscountPrice = _discountPrice;
        }
    }

    public static class DiscountCalculator
    {
        //Final calculation of the dicount based on promotions
        public static decimal GetTotalPrice(Order order, Discount discount)
        {
            try
            {
                decimal d = 0M;
                //club all the ordered products of same ids based on the key which is similar to the discount product ids
                var productCount = order.Products
                    .GroupBy(x => x.Id)
                    .Where(grp => discount.ProductInfo.Any(y => grp.Key == y.Key && grp.Count() >= y.Value))
                    .Select(grp => grp.Count())
                    .Sum();
                int discountCount = discount.ProductInfo.Sum(kvp => kvp.Value);

                //fetch the price of individual product
                var productPrice = order.Products.Find(x => x.Id == discount.ProductInfo.Keys.FirstOrDefault()).Price;
                d = (productCount / discountCount) * discount.DiscountPrice + (productCount % discountCount * productPrice);

                return d;
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("There has been some mistake. Please try again.");
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FetchJsonData obj = new FetchJsonData();
            //fetch data from json
            var inputs = obj.FetchData();
            if (inputs.Promotions == null)
            {
                Console.WriteLine("There is No Promotions.");
            }
            else if (inputs.OrdersPlaced == null)
            {
                Console.WriteLine("There is No Orders Placed.");
            }
            else
            {
                try
                {
                    List<Discount> promotions = new List<Discount>();
                    List<Promotion> promotionList = inputs.Promotions;
                    for (int dict = 0; dict < promotionList.Count; dict++)
                    {
                        //create dictionaries based on the product id used in discounts
                        Dictionary<String, int> dictionary = new Dictionary<String, int>();
                        if (promotionList[dict].ProductId.Contains("&"))
                        {
                            //splitting for dictionary creation for joint discount price eg/.C and D (Keeping the 2 in same)
                            var arrayOfIds = promotionList[dict].ProductId.Split('&');
                            for (int productids = 0; productids < arrayOfIds.Length; productids++)
                            {
                                dictionary.Add(arrayOfIds[productids].Trim(), promotionList[dict].Count);
                            }
                        }
                        else
                        {
                            dictionary.Add(promotionList[dict].ProductId, promotionList[dict].Count);
                        }
                        promotions.Add(new Discount(dict, dictionary, inputs.Promotions[dict].DiscountPrice));
                    }

                    // formating orders from the json file for further usage
                    List<Order> orders = new List<Order>();
                    for (int i = 0; i < inputs.OrdersPlaced.Count; i++)
                    {
                        List<Product> list = new List<Product>();
                        for (int j = 0; j < inputs.OrdersPlaced[i].Products.Count; j++)
                        {
                            for (int l = 0; l < inputs.OrdersPlaced[i].Products[j].Count; l++)
                            {
                                list.Add(new Product(inputs.OrdersPlaced[i].Products[j].ProductId));
                            }
                        }
                        Order order = new Order(i + 1, list);
                        orders.Add(order);
                    }

                    foreach (Order ord in orders)
                    {
                        //select individual orders and send for final result calculation
                        List<decimal> promoprices = promotions
                            .Select(promo => DiscountCalculator.GetTotalPrice(ord, promo))
                            .ToList();
                        // price * no of products
                        decimal actualPrice = ord.Products.Sum(x => x.Price);
                        decimal discountedPrice = promoprices.Sum();                        
                        Console.WriteLine($"OrderID: {ord.OrderID} => Without Discount : {actualPrice.ToString("0.00")} | Discount: {discountedPrice.ToString("0.00")} | Final Cost : {(actualPrice - discountedPrice).ToString("0.00")}");
                    }
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }
    }


}
