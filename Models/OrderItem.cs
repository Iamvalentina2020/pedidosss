using System.ComponentModel.DataAnnotations;

namespace pedidosss.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int ProductoId { get; set; }
        public Product Producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Subtotal { get; set; }
    }
}
