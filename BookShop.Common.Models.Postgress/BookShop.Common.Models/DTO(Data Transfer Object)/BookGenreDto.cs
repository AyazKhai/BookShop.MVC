using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_
{
    public class BookGenreDto
    {
        [Required]
        public int? BookId { get; set; }

        [Required]
        public int? GenreId { get; set; }
    }
}
