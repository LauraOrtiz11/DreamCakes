using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Admin
{
    public class AdminImageDto
    {
        public int ID_Image { get; set; }
        public int ID_Product { get; set; }
        public string ImgName { get; set; }
        public string ImgUrl { get; set; }
        public int Response { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}