using System.ComponentModel.DataAnnotations;


namespace pedidosss.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string ClienteNombre { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string Estado { get; set; } = "Pendiente";

        public decimal Total { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
