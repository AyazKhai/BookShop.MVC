using BookShop.Common.Models.Postgress.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BookShop.Common.Models.Models;

[Table("books")]
public partial class Book
{
    [Key]
    [Column("bookid")]
    public int BookId { get; set; }

    [Required]
    [Column("title")]
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

    [Column("stockquantity")]
    public int? StockQuantity { get; set; }

    [Column("description")]
    public string? BookDescription { get; set; }

    [Column("imagelinks", TypeName = "jsonb")]
    public List<ImageLink>? Imagelinks { get; set; }

    //[ForeignKey("Authorid")]
    //[InverseProperty("Books")]
    //public virtual Author? Author { get; set; }

    [InverseProperty("Book")]
    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    //been changed from BookGenre to Genre
    [InverseProperty("Book")]
    public virtual ICollection<Genre> Bookgenres { get; set; } = new List<Genre>();

    [ForeignKey("Publisherid")]
    [InverseProperty("Books")]
    public  Publisher? Publisher { get; set; }

    [InverseProperty("Book")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [JsonIgnore]
    public List<IFormFile> ImageFiles { get; set; }
}
