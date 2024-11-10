using BookShop.Common.Models.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookShop.MVC.Models
{
    [Keyless]
    public class CartItem
    {
        
        //public int CartItemId { get; set; }
        public int ProductId { get; set; }

        public int BooktId { get; set; }//
       public string Title { get; set; }
        //public Book Book { get; set; }
       
        public string AuthorName { get; set; }//
        
        public string ImageStr { get; set; }//
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool isWrapped { get; set; }
        public bool isStamped { get; set; }


        // Внешний ключ для связи с OrderInfo
        //public int OrderInfoId { get; set; }
        //public OrderInfo OrderInfo { get; set; }
    }

}
