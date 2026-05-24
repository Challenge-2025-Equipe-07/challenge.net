using System.ComponentModel.DataAnnotations;

namespace VetiWebApplication.Models
{
    public class Tutor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string NmTutor { get; set; }

        [Required(ErrorMessage = "CPF é obrigatório.")]
        public string DsCpf { get; set; }
        public string DsEmail { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório.")]
        public string DsTelefone { get; set; }

        //Um tutor pode ter vários pets
        public ICollection<Pet> Pets { get; set; }

    }
}
