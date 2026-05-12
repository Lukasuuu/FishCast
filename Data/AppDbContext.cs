using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FishCast.Models;

namespace FishCast.Data
{
    // Estende IdentityDbContext para herdar as tabelas de autenticação do ASP.NET Identity
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tabelas da aplicação (as tabelas de Identity são herdadas automaticamente)
        public DbSet<Captura> Capturas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Peixe> Peixes { get; set; }
        public DbSet<Seguidor> Seguidores { get; set; }
        public DbSet<CapturaLike> CapturaLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Configurações adicionais do modelo e relações entre entidades
        {
            base.OnModelCreating(modelBuilder);

            // A relação Seguidor tem duas FK para ApplicationUser (quem segue / quem é seguido).
            // DeleteBehavior.Restrict evita cascade delete em ciclos, que o SQL Server não permite.
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

            // Índice composto único:
            modelBuilder.Entity<Seguidor>()
                .HasIndex(s => new { s.SeguidorId, s.SeguidoId })
                .IsUnique(); // impede seguir o mesmo utilizador duas vezes

            modelBuilder.Entity<CapturaLike>()
                .HasOne(cl => cl.Captura) // HasOne e WithMany para configurar a relação entre CapturaLike e Captura
                .WithMany(c => c.Likes)
                .HasForeignKey(cl => cl.CapturaId)
                .OnDelete(DeleteBehavior.Cascade);  // apagar captura remove os seus likes

            modelBuilder.Entity<CapturaLike>()
                .HasOne(cl => cl.Utilizador) // HasOne e WithMany para configurar a relação entre CapturaLike e ApplicationUser
                .WithMany(u => u.Likes)
                .HasForeignKey(cl => cl.UtilizadorId)  // FK para ApplicationUser
                .OnDelete(DeleteBehavior.Restrict);   // impedir cascade delete para evitar apagar utilizadores acidentalmente

            // Índice composto único:
            modelBuilder.Entity<CapturaLike>()
                .HasIndex(cl => new { cl.CapturaId, cl.UtilizadorId })
                .IsUnique();// impede dar like duas vezes na mesma captura
        }
    }
}
