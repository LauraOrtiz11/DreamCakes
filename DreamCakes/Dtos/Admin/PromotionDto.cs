using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace DreamCakes.Dtos.Admin
{
    public class PromotionDto : IValidatableObject
    {
        public int ID_Prom { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NameProm { get; set; }

        [Required(ErrorMessage = "Debe ingresar un porcentaje de descuento.")]
        [Range(0.01, 100.0, ErrorMessage = "Ingrese un porcentaje válido entre 0.01 y 100.")]
        [RegularExpression(@"^\d{1,3}(?:,\d+)?$", ErrorMessage = "El porcentaje de descuento debe usarse con una coma (,) como separador decimal.")]
        [Display(Name = "Porcentaje de descuento")]
        public decimal DiscountPer { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool StateProm { get; set; }
        public string DescriProm { get; set; }

        public int Response { get; set; }
        public string Message { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "La fecha de fin no puede ser anterior a la fecha de inicio.",
                    new[] { "EndDate" }
                );
            }
        }
    }
}