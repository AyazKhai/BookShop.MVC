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
    public class GenreDto
    {

        [Required]
        [StringLength(255)]
        public string GenresName { get; set; } = null!;

        public string? Description { get; set; }

        
    }
}
