using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DreamCakes.Dtos
{
    public class ProductDto
    {
        public int ID_Producto { get; set; }
        public int ID_Categoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string NombreCategoria { get; set; }
        public double PuntuacionPromedio { get; set; }
    }
}