using Microsoft.AspNetCore.Identity;

namespace BookShop.MVC.Models
{
    public class ShopUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty; 
        public string LastName { get; set; } = string.Empty;


    }
}
