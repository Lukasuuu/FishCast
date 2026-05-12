namespace FishCast.Models
{
    // Tabela de junção para a relação many-to-many de seguidores.
    // Um índice único (SeguidorId, SeguidoId) é definido em AppDbContext.OnModelCreating.
    public class Seguidor
    {
        public int Id { get; set; }

        public string SeguidorId { get; set; } = string.Empty;     // quem segue
        public ApplicationUser? Seguidor_Nav { get; set; }

        public string SeguidoId { get; set; } = string.Empty;      // quem é seguido
        public ApplicationUser? Seguido_Nav { get; set; }

        public DateTime DataSeguimento { get; set; } = DateTime.Now;
    }
}
