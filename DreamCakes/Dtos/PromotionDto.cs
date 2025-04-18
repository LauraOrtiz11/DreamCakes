using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos
{
    public class PromotionDto
    {
        public int ID_Promocion { get; set; }
        public string Nombre_Prom { get; set; }
        public string Descrip_Prom { get; set; }
        public decimal Porc_Desc { get; set; }
        public DateTime Fecha_Ini { get; set; }
        public DateTime Fecha_Fin { get; set; }

        public int Estado { get; set; }

        public int Response { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}