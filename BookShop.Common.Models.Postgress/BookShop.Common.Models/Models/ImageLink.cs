using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.Models.Postgress.Models
{
    public class ImageLink
    {
        public string Url { get; set; }
        public string Format { get; set; }
        public int Size { get; set; }
    }

}
