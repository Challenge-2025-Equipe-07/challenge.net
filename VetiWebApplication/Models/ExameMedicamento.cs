namespace VetiWebApplication.Models
{
    // Tabela de relação entre Exame e Medicamento
    public class ExameMedicamento
    {
        // Chave composta: ExameId + MedicamentoId
        public int ExameId { get; set; }
        public Exame Exame { get; set; }

        public int MedicamentoId { get; set; }
        public Medicamento Medicamento { get; set; }

        public int QtMedicamento { get; set; }
    }
}
