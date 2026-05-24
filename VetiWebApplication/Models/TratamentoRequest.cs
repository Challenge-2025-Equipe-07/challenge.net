namespace VetiWebApplication.Models
{
    public class TratamentoRequest
    {
        public string DsDiagnostico { get; set; }
        public DateOnly DtInicio { get; set; }
        public DateOnly? DtRetornoPrevisto { get; set; }
        public string? DsObservacao { get; set; }
        public int PetId { get; set; }
    }
}