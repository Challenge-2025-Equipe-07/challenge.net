using System.ComponentModel.DataAnnotations;

namespace VetiWebApplication.Models
{
    public class Tratamento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Diagnóstico é obrigatório.")]
        public string DsDiagnostico { get; set; }

        [Required(ErrorMessage = "Data de início é obrigatória.")]
        public DateOnly DtInicio { get; set; }

        public DateOnly? DtRetornoPrevisto { get; set; } // opcional

        public string? DsObservacao { get; set; } // observações gerais

        // FK para Pet
        [Required(ErrorMessage = "PetId é obrigatório.")]
        public int PetId { get; set; }
        public Pet? Pet { get; set; }

        // Um tratamento pode ter vários medicamentos
        public ICollection<TratamentoMedicamento>? TratamentoMedicamentos { get; set; }
    }
}