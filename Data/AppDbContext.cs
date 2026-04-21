using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FishCast.Models;

namespace FishCast.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Captura> Capturas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Peixe> Peixes { get; set; }
        public DbSet<Seguidor> Seguidores { get; set; }
        public DbSet<CapturaLike> CapturaLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relação Seguidor (self-referencing many-to-many)
            modelBuilder.Entity<Seguidor>()
                .HasOne(s => s.Seguidor_Nav)
                .WithMany(u => u.Seguidos)
                .HasForeignKey(s => s.SeguidorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Seguidor>()
                .HasOne(s => s.Seguido_Nav)
                .WithMany(u => u.Seguidores)
                .HasForeignKey(s => s.SeguidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice único para evitar seguir duas vezes o mesmo utilizador
            modelBuilder.Entity<Seguidor>()
                .HasIndex(s => new { s.SeguidorId, s.SeguidoId })
                .IsUnique();

            // Configurar relação CapturaLike
            modelBuilder.Entity<CapturaLike>()
                .HasOne(cl => cl.Captura)
                .WithMany(c => c.Likes)
                .HasForeignKey(cl => cl.CapturaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CapturaLike>()
                .HasOne(cl => cl.Utilizador)
                .WithMany(u => u.Likes)
                .HasForeignKey(cl => cl.UtilizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice único para evitar likes duplicados
            modelBuilder.Entity<CapturaLike>()
                .HasIndex(cl => new { cl.CapturaId, cl.UtilizadorId })
                .IsUnique();
        }
    }
}
