using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos.Client
{
    public class ReviewRequestDto
    {
        public int ProductID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

}