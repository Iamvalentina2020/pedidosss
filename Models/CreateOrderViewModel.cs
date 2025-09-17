using GestionPedidos.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace GestionPedidos.Models.ViewModels
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "El cliente es requerido")]
        public int CustomerId { get; set; }

        public string Notes { get; set; } = string.Empty;

        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();

        public IEnumerable<User> Customers { get; set; } = new List<User>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        public decimal EstimatedTotal { get; set; }
    }

    public class OrderItemViewModel
    {
        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }
}