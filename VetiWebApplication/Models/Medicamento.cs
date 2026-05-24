using System.ComponentModel.DataAnnotations;

namespace VetiWebApplication.Models
{
    public class Medicamento
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Nome do medicamento é obrigatório.")]
        public string NmMedicamento { get; set; }


        [Required(ErrorMessage = "Dosagem é obrigatória.")]
        public string DsDosagem { get; set; }


        [Required(ErrorMessage = "Frequência é obrigatória.")]
        public string DsFrequencia { get; set; }

        //Um medicamento pode estar em vários exames
        public ICollection<ExameMedicamento> ExameMedicamentos { get; set; }
    }
}
