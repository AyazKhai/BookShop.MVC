using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_
{
    public class AuthorDto
    {
        [Required]
        [StringLength(255)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string LastName { get; set; } = null!;


        public string? Bio { get; set; }

    }
}
