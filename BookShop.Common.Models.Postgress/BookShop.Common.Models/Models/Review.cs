using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace BookShop.Common.Models.Models;

[Table("reviews")]
public partial class Review
{
    [Key]
    [Column("reviewid")]
    public int ReviewId { get; set; }

    [Required]
    [Column("customerid")]
    public int? CustomerId { get; set; }

    [Required]
    [Column("bookid")]
    public int? BookId { get; set; }

    [Required]
    [Column("rating")]
    public int? Rating { get; set; }


    [Column("comment")]
    public string? CommentText { get; set; }


    [Column("reviewdate")]
    public DateTime ReviewDate { get; set; }

    //[ForeignKey("Bookid")]
    //[InverseProperty("Reviews")]
    //public virtual Book? Book { get; set; }

    //[ForeignKey("Customerid")]
    //[InverseProperty("Reviews")]
    //public virtual Customer? Customer { get; set; }
}
