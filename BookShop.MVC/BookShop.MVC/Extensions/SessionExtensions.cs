using BookShop.MVC.Models;
using System.Text.Json;

namespace BookShop.MVC.Extensions
{
    public static class SessionExtensions
    {
        private const string CartSessionKey = "Cart";

        public static void SetCart(this ISession session, List<CartItem> cart)
        {
            session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public static List<CartItem> GetCart(this ISession session)
        {
            var cartData = session.GetString(CartSessionKey);
            return cartData == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartData);
        }
    }
}
