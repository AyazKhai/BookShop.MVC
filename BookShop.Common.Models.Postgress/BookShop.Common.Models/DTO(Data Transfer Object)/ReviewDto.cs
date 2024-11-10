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
    public class ReviewDto
    {

        [Required]
        public int? CustomerId { get; set; }

        [Required]
        public int? BookId { get; set; }

        [Required]
        public int? Rating { get; set; }

        public string? CommentText { get; set; }

        //public DateOnly ReviewDate { get; set; }

        //[ForeignKey("Bookid")]
        //[InverseProperty("Reviews")]
        //public virtual Book? Book { get; set; }

        //[ForeignKey("Customerid")]
        //[InverseProperty("Reviews")]
        //public virtual Customer? Customer { get; set; }
    }
}
