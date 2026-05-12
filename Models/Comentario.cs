namespace FishCast.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public int CapturaId { get; set; }
        public Captura? Captura { get; set; }
        public string? UtilizadorId { get; set; }
        public ApplicationUser? Utilizador { get; set; }
        public string? Texto { get; set; }
        public DateTime DataComentario { get; set; } = DateTime.Now;
    }
}
