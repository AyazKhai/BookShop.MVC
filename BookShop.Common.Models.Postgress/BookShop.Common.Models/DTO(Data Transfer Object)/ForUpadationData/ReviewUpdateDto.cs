using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_.ForUpadationData
{
    public class ReviewUpdateDto
    {
        [Required]
        public int ReviewId { get; set; }
        [Required]
        public int? Rating { get; set; }

        public string? CommentText { get; set; }
    }
}
