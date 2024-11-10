using BookShop.Common.Models.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BookShop.MVC.Models
{
    public class OrderInfo
    {
        [Key]
        public int OrderId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Поле для хранения коллекции книг в формате JSON
        public string BooksJson { get; set; }

        [NotMapped]
        public ICollection<CartItem> CartItems { get; set; } // Это поле не будет добавлено в БД

        
       

        // Метод для десериализации BooksJson обратно в CartItems
        public void DeserializeCartItems()
        {
            CartItems = string.IsNullOrEmpty(BooksJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(BooksJson);
        }
        // Метод для сериализации CartItems в BooksJson
        public void SerializeCartItems()
        {
            // Расчет стоимости без учета доставки
            //TotalPriceWithoutShipping = CalculateTotalPriceWithoutShipping();

            // Расчет стоимости с учетом доставки
            TotalPrice = CalculateTotalPriceWithShipping();

            // Сериализация товаров
            BooksJson = JsonSerializer.Serialize(CartItems);
        }

        // Метод для подсчета стоимости с доставкой
        public decimal CalculateTotalPriceWithShipping()
        {
            decimal itemsTotalPrice = CartItems.Sum(item => item.Price * item.Quantity);
            decimal shippingCost = 0;

            if (ShippingMethod == "Courier" && itemsTotalPrice <= 500)
            {
                shippingCost = 15;
            }

            return itemsTotalPrice + shippingCost;
        }

        // Метод для подсчета стоимости без доставки
        public decimal CalculateTotalPriceWithoutShipping()
        {
            return CartItems.Sum(item => item.Price * item.Quantity);
        }
    }
}
