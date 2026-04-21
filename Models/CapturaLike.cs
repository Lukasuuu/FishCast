namespace FishCast.Models
{
    public class CapturaLike
    {
        public int Id { get; set; }

        public int CapturaId { get; set; }
        public Captura? Captura { get; set; }

        public string UtilizadorId { get; set; } = string.Empty;
        public ApplicationUser? Utilizador { get; set; }
    }
}
