using Microsoft.EntityFrameworkCore;
using Backend.Restaurant.Models;

namespace Backend.Restaurant.Data
{
    public class AppData : DbContext
    {
        public AppData(DbContextOptions<AppData> options) : base(options)
        {
        }

        // Definición de las tablas
        public DbSet<User> Users { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Profil> Profils { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Lounge> Lounges { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<SmallBox> SmallBoxes { get; set; }
        public DbSet<CashMovement> CashMovements { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<SalesNote> SalesNotes { get; set; }
        public DbSet<PageAccount> PageAccounts { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profil)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.ProfilId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones Table
            modelBuilder.Entity<Table>()
                .HasOne(t => t.Lounge)
                .WithMany(l => l.Tables)
                .HasForeignKey(t => t.LoungeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Worker)
                .WithMany(w => w.Orders)
                .HasForeignKey(o => o.WorkerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.PaymentMethod)
                .WithMany(pm => pm.Orders)
                .HasForeignKey(o => o.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones Reserve
            modelBuilder.Entity<Reserve>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reserves)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Reserve>()
                .HasOne(r => r.Table)
                .WithMany(t => t.Reserves)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Reserve>()
                .HasOne(r => r.Worker)
                .WithMany(w => w.Reserves)
                .HasForeignKey(r => r.WorkerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones SmallBox
            modelBuilder.Entity<SmallBox>()
                .HasOne(sb => sb.User)
                .WithMany(u => u.SmallBoxes)
                .HasForeignKey(sb => sb.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones CashMovement
            modelBuilder.Entity<CashMovement>()
                .HasOne(cm => cm.SmallBox)
                .WithMany(sb => sb.CashMovements)
                .HasForeignKey(cm => cm.SmallBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de relaciones Invoice
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Order)
                .WithMany(o => o.Invoices)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones Ticket
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Order)
                .WithMany(o => o.Tickets)
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de relaciones SalesNote
            modelBuilder.Entity<SalesNote>()
                .HasOne(sn => sn.Order)
                .WithMany(o => o.SalesNotes)
                .HasForeignKey(sn => sn.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de precisión decimal para propiedades monetarias
            ConfigureDecimalPrecision(modelBuilder);

            // Configuración de índices para mejorar rendimiento
            ConfigureIndexes(modelBuilder);
        }

        private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            // Order
            modelBuilder.Entity<Order>()
                .Property(o => o.SubTotal)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(o => o.Discount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(o => o.Tax)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            // OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.SubTotal)
                .HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Total)
                .HasPrecision(18, 2);

            // Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Invoice
            modelBuilder.Entity<Invoice>()
                .Property(i => i.SubTotal)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>()
                .Property(i => i.IGV)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>()
                .Property(i => i.Total)
                .HasPrecision(18, 2);

            // Ticket
            modelBuilder.Entity<Ticket>()
                .Property(t => t.SubTotal)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Ticket>()
                .Property(t => t.Discount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Ticket>()
                .Property(t => t.Total)
                .HasPrecision(18, 2);

            // SalesNote
            modelBuilder.Entity<SalesNote>()
                .Property(sn => sn.SubTotal)
                .HasPrecision(18, 2);
            modelBuilder.Entity<SalesNote>()
                .Property(sn => sn.Discount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<SalesNote>()
                .Property(sn => sn.Total)
                .HasPrecision(18, 2);

            // Reserve
            modelBuilder.Entity<Reserve>()
                .Property(r => r.Amount)
                .HasPrecision(18, 2);

            // PageAccount
            modelBuilder.Entity<PageAccount>()
                .Property(pa => pa.Amount)
                .HasPrecision(18, 2);

            // SmallBox
            modelBuilder.Entity<SmallBox>()
                .Property(sb => sb.InitialAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<SmallBox>()
                .Property(sb => sb.FinalAmount)
                .HasPrecision(18, 2);

            // CashMovement
            modelBuilder.Entity<CashMovement>()
                .Property(cm => cm.Amount)
                .HasPrecision(18, 2);

            // Worker
            modelBuilder.Entity<Worker>()
                .Property(w => w.SalaryWorker)
                .HasPrecision(18, 2);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Índices para User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Índices para Order
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate);
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);

            // Índices para Worker
            modelBuilder.Entity<Worker>()
                .HasIndex(w => w.DNI)
                .IsUnique();

            // Índices para Invoice
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            // Índices para Ticket
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.TicketNumber)
                .IsUnique();

            // Índices para SalesNote
            modelBuilder.Entity<SalesNote>()
                .HasIndex(sn => sn.NoteNumber)
                .IsUnique();

            // Índices para Reserve
            modelBuilder.Entity<Reserve>()
                .HasIndex(r => r.ReservationDate);

            // Índices para Product
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.NameProduct);

            // Índices para Category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.NameCategory);
        }
    }
}
