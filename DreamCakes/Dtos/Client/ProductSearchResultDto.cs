using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DreamCakes.Dtos.Client
{
    public class ProductSearchResultDto
    {
        public int ID_Producto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public decimal AvgRating { get; set; }
        public int ID_Categoria { get; set; }
        public string Nom_Categ { get; set; }
        public string Descrip_Categ { get; set; }
        public bool Estado { get; set; }
    }
}