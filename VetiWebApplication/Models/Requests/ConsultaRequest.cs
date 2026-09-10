namespace VetiWebApplication.Models.Requests
{
    public class ConsultaRequest
    {
        public DateOnly DtConsulta { get; set; }
        public string TpEvento { get; set; }
        public int Notificar { get; set; }
        public int PetId { get; set; }
        public int VeterinarioId { get; set; }
    }
}