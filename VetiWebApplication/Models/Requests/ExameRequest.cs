namespace VetiWebApplication.Models.Requests
{
    public class ExameRequest
    {
        public string DsDocumento { get; set; }
        public DateTime DtRealizacao { get; set; }
        public string DsDiagnostico { get; set; }
        public int ConsultaId { get; set; }
    }
}
