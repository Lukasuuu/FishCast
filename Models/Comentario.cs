namespace FishCast.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        // O Id da captura a que o comentário pertence
        public int CapturaId { get; set; }
        public Captura? Captura { get; set; }

        // O Id do utilizador que fez o comentário
        public string? UtilizadorId { get; set; }
        public ApplicationUser? Utilizador { get; set; }

        public string? Texto { get; set; }

        public DateTime DataComentario { get; set; } = DateTime.Now;
    }
}
