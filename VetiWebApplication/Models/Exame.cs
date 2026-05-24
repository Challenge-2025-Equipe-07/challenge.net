using System.ComponentModel.DataAnnotations;

namespace VetiWebApplication.Models
{
    public class Exame
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Documento é obrigatório.")]
        public string DsDocumento { get; set; }

        [Required(ErrorMessage = "Data é obrigatória.")]
        public DateTime DtRealizacao { get; set; }

        [Required(ErrorMessage = "Diagnóstico é obrigatório.")]
        public string DsDiagnostico { get; set; }

        //FK para Consulta

        [Required(ErrorMessage = "Id da consulta é obrigatório.")]
        public int ConsultaId { get; set; }
        public Consulta Consulta { get; set; }

        //Um exame pode ter vários medicamentos
        public ICollection<ExameMedicamento> ExameMedicamentos { get; set; }
    }
}
