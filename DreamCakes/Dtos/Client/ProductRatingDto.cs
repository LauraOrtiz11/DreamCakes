using System.ComponentModel.DataAnnotations;

namespace DreamCakes.Dtos.Client
{
    public class ProductRatingDto
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "El ID de usuario es requerido")]
        public int UsuarioID { get; set; }  // Cambiado de ClientID a UsuarioID

        [Required(ErrorMessage = "La puntuación es requerida")]
        [Range(1, 5, ErrorMessage = "La puntuación debe ser entre 1 y 5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
        public string Comment { get; set; } = string.Empty;

        public int Response { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}