//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DreamCakes.Repositories.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PROMOCION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PROMOCION()
        {
            this.DETALLE_PEDIDO = new HashSet<DETALLE_PEDIDO>();
            this.PROMOCION_PRODUCTO = new HashSet<PROMOCION_PRODUCTO>();
        }
    
        public int ID_Promocion { get; set; }
        public string Nombre_Prom { get; set; }
        public string Descrip_Prom { get; set; }
        public decimal Porc_Desc { get; set; }
        public System.DateTime Fecha_Ini { get; set; }
        public System.DateTime Fecha_Fin { get; set; }
        public bool Estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLE_PEDIDO> DETALLE_PEDIDO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PROMOCION_PRODUCTO> PROMOCION_PRODUCTO { get; set; }
    }
}
