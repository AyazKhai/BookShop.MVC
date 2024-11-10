using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Common.Models.Models;

[Table("authors")]
public partial class Author
{
    [Key]
    [Column("authorid")]
    public int AuthorId { get; set; }

    [Required]
    [Column("firstname")]
    [StringLength(255)]
    public string FirstName { get; set; } = null!;

    [Required]
    [Column("lastname")]
    [StringLength(255)]
    public string LastName { get; set; } = null!;

    [Column("bio")]
    public string? Bio { get; set; }

    //[InverseProperty("Author")]
    //public virtual ICollection<Bookauthor> Bookauthors { get; set; } = new List<Bookauthor>();

    //[InverseProperty("Author")]
    //public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
