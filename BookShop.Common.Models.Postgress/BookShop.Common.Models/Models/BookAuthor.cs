using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Common.Models.Models;

[Table("bookauthors")]
public partial class BookAuthor
{
    [Key]
    [Column("bookauthorid")]
    public int BookauthorId { get; set; }

    [Required]
    [Column("bookid")]
    public int? BookId { get; set; }

    [Required]
    [Column("authorid")]
    public int? AuthorId { get; set; }

    //[Required]
    //[ForeignKey("Authorid")]
    //[InverseProperty("Bookauthors")]
    //public virtual Author? Author { get; set; }

    //[ForeignKey("Bookid")]
    //[InverseProperty("Bookauthors")]
    //public virtual Book? Book { get; set; }
}
