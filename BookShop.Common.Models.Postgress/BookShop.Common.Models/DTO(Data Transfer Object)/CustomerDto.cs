using BookShop.Common.Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_
{
    public class CustomerDto
    {

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }


        public string? Address { get; set; }

        //[InverseProperty("Customer")]
        //public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
