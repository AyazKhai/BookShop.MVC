using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Common.Models.Models;

[Table("bookgenres")]
public partial class BookGenre
{
    [Key]
    [Column("bookgenreid")]
    public int BookgenreId { get; set; }

    [Required]
    [Column("bookid")]
    public int? BookId { get; set; }

    [Required]
    [Column("genreid")]
    public int? GenreId { get; set; }

    //[ForeignKey("Bookid")]
    //[InverseProperty("Bookgenres")]
    //public virtual Book? Book { get; set; }

    //[ForeignKey("Genreid")]
    //[InverseProperty("Bookgenres")]
    //public virtual Genre? Genre { get; set; }
}
