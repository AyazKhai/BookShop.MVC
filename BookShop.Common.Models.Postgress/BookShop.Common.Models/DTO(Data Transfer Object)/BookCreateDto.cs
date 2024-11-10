using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using BookShop.Common.Models.Postgress.Models;

namespace BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_
{
    public class BookCreateDto
    {

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;
        [Required]
        [Column("publisherid")]
        public int? PublisherId { get; set; }


        [Column("price")]
        public decimal Price { get; set; }

        [Column("isbn")]
        [StringLength(13)]
        public string Isbn { get; set; } = null!;


        public int? StockQuantity { get; set; }

        [Column("description")]
        public string? BookDescription { get; set; }

        [Column("imagelinks", TypeName = "json")] 
        public List<ImageLink>? Imagelinks { get; set; }
    }
}
