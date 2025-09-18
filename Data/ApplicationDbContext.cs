using GestionPedidos.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace pedidosss.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasConversion<int>();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Total).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasConversion<int>();

                entity.HasOne(e => e.Customer)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.Subtotal).HasPrecision(18, 2);

                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Administrador", Email = "admin@sistema.com", Password = "admin123", Role = UserRole.Admin, CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new User { Id = 2, Name = "Juan Pérez", Email = "juan@email.com", Password = "cliente123", Role = UserRole.Customer, CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new User { Id = 3, Name = "María García", Email = "maria@email.com", Password = "empleado123", Role = UserRole.Employee, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new User { Id = 4, Name = "Carlos López", Email = "carlos@email.com", Password = "cliente123", Role = UserRole.Customer, CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new User { Id = 5, Name = "Ana Martínez", Email = "ana@email.com", Password = "cliente123", Role = UserRole.Customer, CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new User { Id = 6, Name = "Pedro Rodríguez", Email = "pedro@email.com", Password = "empleado123", Role = UserRole.Employee, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new User { Id = 7, Name = "Laura Sánchez", Email = "laura@email.com", Password = "cliente123", Role = UserRole.Customer, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new User { Id = 8, Name = "Miguel Torres", Email = "miguel@email.com", Password = "cliente123", Role = UserRole.Customer, CreatedAt = DateTime.UtcNow.AddDays(-8) }
            );

            modelBuilder.Entity<Product>().HasData(

                new Product { Id = 1, Name = "Laptop Dell Inspiron 15", Description = "Laptop Dell Inspiron 15 con procesador Intel i5, 8GB RAM, 256GB SSD", Price = 850.00m, Stock = 10, Category = "Electrónicos", CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new Product { Id = 2, Name = "Mouse Inalámbrico", Description = "Mouse inalámbrico ergonómico con sensor óptico de alta precisión", Price = 25.99m, Stock = 50, Category = "Accesorios", CreatedAt = DateTime.UtcNow.AddDays(-28) },
                new Product { Id = 3, Name = "Teclado Mecánico RGB", Description = "Teclado mecánico con iluminación RGB y switches Cherry MX", Price = 89.99m, Stock = 25, Category = "Accesorios", CreatedAt = DateTime.UtcNow.AddDays(-26) },
                new Product { Id = 4, Name = "Monitor LED 24 pulgadas", Description = "Monitor LED Full HD de 24 pulgadas con tecnología IPS", Price = 180.00m, Stock = 15, Category = "Electrónicos", CreatedAt = DateTime.UtcNow.AddDays(-24) },
                new Product { Id = 5, Name = "Smartphone Samsung Galaxy", Description = "Smartphone con pantalla de 6.4 pulgadas, 128GB almacenamiento", Price = 650.00m, Stock = 8, Category = "Electrónicos", CreatedAt = DateTime.UtcNow.AddDays(-22) },
                new Product { Id = 6, Name = "Auriculares Bluetooth", Description = "Auriculares inalámbricos con cancelación de ruido activa", Price = 120.00m, Stock = 30, Category = "Accesorios", CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Product { Id = 7, Name = "Tablet Android 10 pulgadas", Description = "Tablet con pantalla de 10 pulgadas, 4GB RAM, 64GB almacenamiento", Price = 280.00m, Stock = 12, Category = "Electrónicos", CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Product { Id = 8, Name = "Webcam HD", Description = "Cámara web HD 1080p con micrófono incorporado", Price = 45.99m, Stock = 22, Category = "Accesorios", CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Product { Id = 9, Name = "Disco Duro Externo 1TB", Description = "Disco duro portátil USB 3.0 de 1TB para backup y almacenamiento", Price = 75.00m, Stock = 18, Category = "Almacenamiento", CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Product { Id = 10, Name = "Impresora Multifuncional", Description = "Impresora a color con escáner y Wi-Fi integrado", Price = 199.99m, Stock = 7, Category = "Electrónicos", CreatedAt = DateTime.UtcNow.AddDays(-12) },

                new Product { Id = 11, Name = "Silla de Oficina Ergonómica", Description = "Silla de oficina con soporte lumbar y reposabrazos ajustables", Price = 145.00m, Stock = 20, Category = "Mobiliario", CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Product { Id = 12, Name = "Escritorio de Madera", Description = "Escritorio ejecutivo de madera maciza con cajones", Price = 320.00m, Stock = 5, Category = "Mobiliario", CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Product { Id = 13, Name = "Lámpara LED de Escritorio", Description = "Lámpara LED regulable con brazo articulado y carga USB", Price = 38.50m, Stock = 35, Category = "Iluminación", CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Product { Id = 14, Name = "Organizador de Cables", Description = "Kit organizador de cables para escritorio con clips y canaletas", Price = 15.99m, Stock = 60, Category = "Accesorios", CreatedAt = DateTime.UtcNow.AddDays(-4) },
                new Product { Id = 15, Name = "Planta de Escritorio", Description = "Planta suculenta en maceta decorativa ideal para oficina", Price = 12.00m, Stock = 40, Category = "Decoración", CreatedAt = DateTime.UtcNow.AddDays(-2) },

                new Product { Id = 16, Name = "Silla Gaming RGB", Description = "Silla gaming ergonómica con iluminación RGB y soporte cervical", Price = 299.99m, Stock = 8, Category = "Gaming", CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Product { Id = 17, Name = "Mouse Gaming 12000 DPI", Description = "Mouse gaming con sensor de alta precisión y 12 botones programables", Price = 65.00m, Stock = 25, Category = "Gaming", CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Product { Id = 18, Name = "Pad RGB XXL", Description = "Alfombrilla gaming XXL con iluminación RGB y superficie micro-texturizada", Price = 32.99m, Stock = 30, Category = "Gaming", CreatedAt = DateTime.UtcNow.AddDays(-11) },

                new Product { Id = 19, Name = "Cable HDMI 4K", Description = "Cable HDMI de alta velocidad compatible con 4K y HDR", Price = 18.99m, Stock = 3, Category = "Accesorios", CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Product { Id = 20, Name = "Adaptador USB-C Hub", Description = "Hub USB-C con múltiples puertos USB, HDMI y carga PD", Price = 42.00m, Stock = 2, Category = "Accesorios", CreatedAt = DateTime.UtcNow.AddDays(-7) }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, CustomerId = 2, OrderDate = DateTime.UtcNow.AddDays(-15), Status = OrderStatus.Delivered, Total = 875.99m, Notes = "Entrega en horario de oficina" },
                new Order { Id = 2, CustomerId = 4, OrderDate = DateTime.UtcNow.AddDays(-12), Status = OrderStatus.Delivered, Total = 205.99m, Notes = "Cliente satisfecho" },
                new Order { Id = 3, CustomerId = 5, OrderDate = DateTime.UtcNow.AddDays(-8), Status = OrderStatus.Shipped, Total = 650.00m, Notes = "Envío express solicitado" },
                new Order { Id = 4, CustomerId = 7, OrderDate = DateTime.UtcNow.AddDays(-5), Status = OrderStatus.Processing, Total = 332.98m, Notes = "Verificar dirección de entrega" },
                new Order { Id = 5, CustomerId = 8, OrderDate = DateTime.UtcNow.AddDays(-3), Status = OrderStatus.Pending, Total = 158.49m, Notes = "Pedido urgente" },
                new Order { Id = 6, CustomerId = 2, OrderDate = DateTime.UtcNow.AddDays(-1), Status = OrderStatus.Pending, Total = 89.99m, Notes = "Regalo para cumpleaños" }
            );

            modelBuilder.Entity<OrderItem>().HasData(

                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 850.00m, Subtotal = 850.00m },
                new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 1, UnitPrice = 25.99m, Subtotal = 25.99m },


                new OrderItem { Id = 3, OrderId = 2, ProductId = 4, Quantity = 1, UnitPrice = 180.00m, Subtotal = 180.00m },
                new OrderItem { Id = 4, OrderId = 2, ProductId = 2, Quantity = 1, UnitPrice = 25.99m, Subtotal = 25.99m },


                new OrderItem { Id = 5, OrderId = 3, ProductId = 5, Quantity = 1, UnitPrice = 650.00m, Subtotal = 650.00m },


                new OrderItem { Id = 6, OrderId = 4, ProductId = 16, Quantity = 1, UnitPrice = 299.99m, Subtotal = 299.99m },
                new OrderItem { Id = 7, OrderId = 4, ProductId = 18, Quantity = 1, UnitPrice = 32.99m, Subtotal = 32.99m },


                new OrderItem { Id = 8, OrderId = 5, ProductId = 6, Quantity = 1, UnitPrice = 120.00m, Subtotal = 120.00m },
                new OrderItem { Id = 9, OrderId = 5, ProductId = 13, Quantity = 1, UnitPrice = 38.49m, Subtotal = 38.49m },


                new OrderItem { Id = 10, OrderId = 6, ProductId = 3, Quantity = 1, UnitPrice = 89.99m, Subtotal = 89.99m }
            );
        }
    }
}