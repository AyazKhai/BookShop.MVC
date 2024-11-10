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
    public partial class PublisherDto
    {

        [Required]
        [StringLength(255)]
        public string Publishername { get; set; } = null!;

        [StringLength(255)]
        public string? Contactinfo { get; set; }

        //[InverseProperty("Publisher")]
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
