using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookShop.Common.Models.Models;

[Table("genres")]
public partial class Genre
{
    [Key]
    [Column("genreid")]
    public int GenreId { get; set; }

    [Required]
    [Column("genresname")]
    [StringLength(255)]
    public string GenresName { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    //[JsonIgnore]
    //[InverseProperty("Genre")]
    //public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}
