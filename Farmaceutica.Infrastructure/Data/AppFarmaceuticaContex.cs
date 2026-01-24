using Farmaceutica.Core.DTOs;
using Farmaceutica.Core.DTOs.Farmaceutica.Core.DTOs;
using Farmaceutica.Core.Entities;
using Farmaceutica.Core.Temporales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Infrastructure.Data
{
    public class AppFarmaceuticaContex : DbContext
    {
        public AppFarmaceuticaContex(DbContextOptions<AppFarmaceuticaContex> options)
            : base(options)
        {
        }

        // 🔹 DbSets: cada entidad representará una tabla
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<SubCategoria> SubCategorias { get; set; }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<ProductoTemp> ProductoTemps { get; set; }

        // public DbSet<SubCategoriaDTO> SubCategoriaDTOs { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        public DbSet<DetalleCompra> DetalleCompra { get; set; }

        public DbSet<Venta> Ventas { get; set; }

        public DbSet<TokenRecuperacion> TokensRecuperacion { get; set; }

        public DbSet<TotalesVentaDto> TotalesVentaDtos { get; set; }

        

        public DbSet<Cliente> ClientesDto { get; set; }

        //Proceso de Compra / Inventarios
        public DbSet<Compra> Compras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔸 Configuración Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.UsuarioNombre)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.HasIndex(e => e.UsuarioNombre)
                      .IsUnique();

                entity.Property(e => e.Password)
                      .IsRequired();

                entity.Property(e => e.Rol)
                      .IsRequired();

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETDATE()");
            });

            // 🔸 Configuración Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Descripcion)
                      .HasMaxLength(500);

                // Relación con SubCategorias
                entity.HasMany(c => c.SubCategorias)
                      .WithOne(sc => sc.Categoria)
                      .HasForeignKey(sc => sc.CategoriaId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Productos
                entity.HasMany(c => c.Productos)
                      .WithOne(p => p.Categoria)
                      .HasForeignKey(p => p.CategoriaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 🔸 Configuración SubCategoria
            modelBuilder.Entity<SubCategoria>(entity =>
            {
                entity.ToTable("SubCategorias");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Descripcion)
                      .HasMaxLength(500);

                // Relación con Productos
                entity.HasMany(sc => sc.Productos)
                      .WithOne(p => p.SubCategoria)
                      .HasForeignKey(p => p.SubCategoriaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 🔸 Configuración Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.Id);

                entity.Property(p => p.Nombre)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Descripcion)
                      .HasMaxLength(1000);

                entity.Property(p => p.Precio)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(p => p.StockTotal)
                      .HasDefaultValue(0);

                entity.Property(p => p.StockLocal)
                      .HasDefaultValue(0);

                entity.Property(p => p.StockDelivery)
                      .HasDefaultValue(0);

                entity.Property(p => p.Codigo)
                      .HasMaxLength(100);

                entity.Property(p => p.Imagen)
                      .HasMaxLength(500);

                // Relación con DetalleVenta
                entity.HasMany(p => p.DetalleVentas)
                      .WithOne(dv => dv.Producto)
                      .HasForeignKey(dv => dv.ProductoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.DetalleCompras)
                      .WithOne(dv => dv.Producto)
                      .HasForeignKey(dv => dv.ProductoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            

            // 🔸 Configuración ProductoTemp (temporal)
            modelBuilder.Entity<ProductoTemp>(entity =>
            {
                entity.ToTable("ProductoTemp");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Descripcion)
                      .HasMaxLength(1000);

                entity.Property(e => e.Precio)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.StockLocal)
                      .HasDefaultValue(0);

                entity.Property(e => e.StockDelivery)
                      .HasDefaultValue(0);

                entity.Property(e => e.Codigo)
                      .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.Codigo);
            });
            modelBuilder.Entity<Venta>()
               .HasOne(v => v.Cliente)
               .WithMany()
               .HasForeignKey(v => v.ClienteId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Encargado)
                .WithMany()
                .HasForeignKey(v => v.EncargadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TotalesVentaDto>().HasNoKey();
            modelBuilder.Entity<DetalleVentaListarDto>().HasNoKey();
            modelBuilder.Entity<ProductosVendidosDTO>(entity =>
            {
                entity.HasNoKey(); // <-- clave primaria no requerida
                entity.Property(e => e.PorcentajeVendido).HasColumnType("decimal(5,2)"); // opcional
            });

            //Proceso de Compra / Inventarios
            modelBuilder.Entity<Compra>()
               .HasOne(v => v.Proveedor)
               .WithMany()
               .HasForeignKey(v => v.ProveedorId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Compra>()
                .HasOne(v => v.Encargado)
                .WithMany()
                .HasForeignKey(v => v.EncargadoId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TotalesCompraDto>().HasNoKey();

            modelBuilder.Entity<DetalleCompraListarDto>().HasNoKey();

            modelBuilder.Entity<Cliente>().HasNoKey();

            modelBuilder.Entity<DetalleVenta>()
            .Property(d => d.Id)
            .ValueGeneratedOnAdd();
            modelBuilder.Entity<ProductoPrincipalDto>(entity =>
            {
                entity.HasNoKey(); // Esto es importante
                entity.ToView(null); // No está mapeado a una vista
            });

        }


    }
}
