using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Common.Models.Models;

[Table("publishers")]
public partial class Publisher
{
    [Key]
    [Column("publisherid")]
    public int? Publisherid { get; set; }

    [Required]
    [Column("publishername")]
    [StringLength(255)]
    public string Publishername { get; set; } = null!;

    [Column("contactinfo")]
    [StringLength(255)]
    public string? Contactinfo { get; set; }

    [InverseProperty("Publisher")]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
