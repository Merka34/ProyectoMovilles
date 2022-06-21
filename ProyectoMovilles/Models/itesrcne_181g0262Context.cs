using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ProyectoMovilles.Models
{
    public partial class itesrcne_181g0262Context : DbContext
    {
        public itesrcne_181g0262Context()
        {
        }

        public itesrcne_181g0262Context(DbContextOptions<itesrcne_181g0262Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Celular> Celular { get; set; }
        public virtual DbSet<Dispositivo> Dispositivo { get; set; }
        public virtual DbSet<Nota> Nota { get; set; }
        public virtual DbSet<Perfil> Perfil { get; set; }
        public virtual DbSet<Tipodisco> Tipodisco { get; set; }
        public virtual DbSet<Tipodispositivo> Tipodispositivo { get; set; }
        public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=204.93.216.11;user=itesrcne_daniel;database=itesrcne_181g0262;password=181G0262", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.29-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8");

            modelBuilder.Entity<Celular>(entity =>
            {
                entity.ToTable("celular");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Eliminado).HasColumnType("bit(1)");

                entity.Property(e => e.Marca).HasMaxLength(45);

                entity.Property(e => e.MemoriaRam).HasColumnType("int(2)");

                entity.Property(e => e.Modelo).HasMaxLength(60);

                entity.Property(e => e.Precio).HasPrecision(10);

                entity.Property(e => e.Red).HasMaxLength(45);

                entity.Property(e => e.Timestamp).HasColumnType("timestamp");
            });

            modelBuilder.Entity<Dispositivo>(entity =>
            {
                entity.ToTable("dispositivo");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CapacidadDisco)
                    .HasColumnType("int(11)")
                    .HasColumnName("capacidadDisco");

                entity.Property(e => e.Eliminado)
                    .HasColumnType("bit(1)")
                    .HasColumnName("eliminado")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Marca)
                    .HasMaxLength(20)
                    .HasColumnName("marca");

                entity.Property(e => e.MemoriaRam)
                    .HasColumnType("int(11)")
                    .HasColumnName("memoriaRAM");

                entity.Property(e => e.Modelo)
                    .HasMaxLength(20)
                    .HasColumnName("modelo");

                entity.Property(e => e.Precio)
                    .HasPrecision(10)
                    .HasColumnName("precio");

                entity.Property(e => e.Procesador)
                    .HasMaxLength(45)
                    .HasColumnName("procesador");

                entity.Property(e => e.TarjetaGrafica)
                    .HasMaxLength(45)
                    .HasColumnName("tarjetaGrafica");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");

                entity.Property(e => e.TipoDisco)
                    .HasColumnType("int(11)")
                    .HasColumnName("tipoDisco");

                entity.Property(e => e.TipoDispositivo)
                    .HasColumnType("int(11)")
                    .HasColumnName("tipoDispositivo");
            });

            modelBuilder.Entity<Nota>(entity =>
            {
                entity.ToTable("nota");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("descripcion");

                entity.Property(e => e.Eliminado)
                    .HasColumnType("bit(1)")
                    .HasColumnName("eliminado");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("titulo");
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.ToTable("perfil");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Carrera)
                    .HasMaxLength(45)
                    .HasColumnName("carrera");

                entity.Property(e => e.Correo)
                    .HasMaxLength(45)
                    .HasColumnName("correo");

                entity.Property(e => e.NoControl)
                    .HasMaxLength(45)
                    .HasColumnName("noControl");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(45)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Tipodisco>(entity =>
            {
                entity.ToTable("tipodisco");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Tipodispositivo>(entity =>
            {
                entity.ToTable("tipodispositivo");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Clave)
                    .HasMaxLength(512)
                    .HasColumnName("clave");

                entity.Property(e => e.NombreUsu)
                    .HasMaxLength(45)
                    .HasColumnName("nombreUsu");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
