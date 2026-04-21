namespace FishCast.Models
{
    public class Seguidor
    {
        public int Id { get; set; }

        // FK ApplicationUser - quem segue
        public string SeguidorId { get; set; } = string.Empty;
        public ApplicationUser? Seguidor_Nav { get; set; }

        // FK ApplicationUser - quem é seguido
        public string SeguidoId { get; set; } = string.Empty;
        public ApplicationUser? Seguido_Nav { get; set; }

        public DateTime DataSeguimento { get; set; } = DateTime.Now;
    }
}
