using EntityFramework;
using Microsoft.EntityFrameworkCore;
public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Cart> Carts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --------------------USER--------------------
        modelBuilder.Entity<User>().HasKey(u => u.UserID); //PK
        modelBuilder.Entity<User>().Property(u => u.UserID).IsRequired().ValueGeneratedOnAdd();

        modelBuilder.Entity<User>().Property(u => u.Username).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<User>().Property(u => u.Password).IsRequired().HasAnnotation("MinLength", 8);
        // .HasAnnotation("MaxLength", 200);
        // .HasAnnotation("RegularExpression", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,32}$");

        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasAnnotation("MinLength", 5)
        .HasAnnotation("MaxLength", 50)
        .HasAnnotation("RegularExpression", @"^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$");

        modelBuilder.Entity<User>().Property(u => u.FirstName).IsRequired().HasMaxLength(20);

        modelBuilder.Entity<User>().Property(u => u.PhoneNumber).HasMaxLength(10);

        modelBuilder.Entity<User>().Property(u => u.Address).HasMaxLength(255);

        // ### Relationship
        modelBuilder.Entity<User>()
        .HasMany(u => u.Orders)
        .WithOne(o => o.User)
        .HasForeignKey(o => o.UserId);

        modelBuilder.Entity<User>()
        .HasMany(u => u.Carts)    
        .WithOne(c => c.User)     
        .HasForeignKey(c => c.UserID); 

        // --------------------Category--------------------
        modelBuilder.Entity<Category>().HasKey(c => c.CategoryID); //PK
        modelBuilder.Entity<Category>().Property(c => c.CategoryID).IsRequired().ValueGeneratedOnAdd();

        modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();

        modelBuilder.Entity<Category>().Property(c => c.Description).HasDefaultValue(string.Empty);

        // ### Relationship
        modelBuilder.Entity<Category>()
        .HasMany(c => c.Products)
        .WithOne(p => p.Category)
        .HasForeignKey(p => p.CategoryId);

        // --------------------Order--------------------
        modelBuilder.Entity<Order>().HasKey(o => o.OrderId); //PK
        modelBuilder.Entity<Order>().Property(o => o.OrderId).IsRequired().ValueGeneratedOnAdd();

        modelBuilder.Entity<Order>().Property(o => o.Status).IsRequired();

        modelBuilder.Entity<Order>().Property(o => o.Payment).IsRequired();

        // ### Relationship Many-To-Many
        modelBuilder.Entity<Order>()
        .HasMany(o => o.Products)
        .WithMany(p => p.Orders)
        .UsingEntity(j => j.ToTable("OrderDetails"));

        // --------------------Product--------------------
        modelBuilder.Entity<Product>().HasKey(p => p.ProductID); //PK
        modelBuilder.Entity<Product>().Property(p => p.ProductID).IsRequired().ValueGeneratedOnAdd();

        modelBuilder.Entity<Product>().Property(p => p.ProductName).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Product>().Property(c => c.Description).HasDefaultValue(string.Empty);

        modelBuilder.Entity<Product>().Property(p => p.Quantity).IsRequired();

        modelBuilder.Entity<Product>().Property(p => p.Price).IsRequired();
        modelBuilder.Entity<Product>().Property(c => c.CreatedAt).HasDefaultValue(DateTime.UtcNow);


        //-------------Cart-------------------- 

        modelBuilder.Entity<Cart>()
        .HasKey(c => new { c.CartId});

        modelBuilder.Entity<Cart>()
        .HasMany(c => c.Products)    
        .WithOne(p => p.Cart)     
        .HasForeignKey(p => p.CartId); 

    }

}
