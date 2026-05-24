namespace VetiWebApplication.Models
{
    public class TratamentoMedicamento
    {
        // Chave composta
        public int TratamentoId { get; set; }
        public Tratamento? Tratamento { get; set; }

        public int MedicamentoId { get; set; }
        public Medicamento? Medicamento { get; set; }

        public int QtMedicamento { get; set; }
        public string? DsInstrucao { get; set; } // ex: "tomar após refeição"
    }
}