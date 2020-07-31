using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace PromotionEngine
{
    /// <summary>
    /// All the promotions 
    /// </summary>
    public class Promotion
    {
        public string ProductId { get; set; }
        public int Count { get; set; }
        public int DiscountPrice { get; set; }
    }
    /// <summary>
    /// Product with the Id and count placed
    /// </summary>
    public class Products
    {
        public string ProductId { get; set; }
        public int Count { get; set; }
    }
    /// <summary>
    /// Orders with the list of all products within it
    /// </summary>
    public class OrdersPlaced
    {
        public int OrderId { get; set; }
        public List<Products> Products { get; set; }
    }
    //Root folder for json formatting
    public class Root
    {
        public List<Promotion> Promotions { get; set; }
        public List<OrdersPlaced> OrdersPlaced { get; set; }
    }

    class FetchJsonData
    {
        public Root FetchData()
        {
            try
            {
                var jsonFile = @"../../DataInputs\Inputs.json";
                var data = File.ReadAllText(jsonFile);
                Root deserialisedData = JsonConvert.DeserializeObject<Root>(data);

                return deserialisedData;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("No file found for orders");
                throw null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
