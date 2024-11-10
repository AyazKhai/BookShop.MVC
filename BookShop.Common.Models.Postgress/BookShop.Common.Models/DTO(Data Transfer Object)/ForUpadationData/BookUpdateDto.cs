using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Common.Models.Postgress.Models;

namespace BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_.ForUpadationData
{
    public class BookUpdateDto
    {
        public int BookId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

       
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

        [Column("imagelinks", TypeName = "jsonb")]
        public List<ImageLink>? Imagelinks { get; set; }
    }
}
