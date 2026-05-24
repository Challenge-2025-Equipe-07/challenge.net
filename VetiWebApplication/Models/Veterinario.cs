using System.ComponentModel.DataAnnotations;

namespace VetiWebApplication.Models
{
    public class Veterinario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        public string DsEmail { get; set; }

        [Required(ErrorMessage = "Senha é obrigatório.")]
        public string DsPassword { get; set; }

        //Um veterinário pode realizar várias consultas
        public ICollection <Consulta> Consultas { get; set; }
    }
}
