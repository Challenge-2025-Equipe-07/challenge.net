using System.ComponentModel.DataAnnotations;

namespace VetiWebApplication.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string NmPet { get; set; }

        [Required(ErrorMessage = "Espécie é obrigatório.")]
        public string DsEspecie { get; set; }

        [Required(ErrorMessage = "Raça é obrigatório.")]
        public string DsRaca { get; set; }

        [Required(ErrorMessage = "Idade é obrigatório.")]
        public int NrIdade { get; set; }

        /// <summary>
        /// Indica se o pet é castrado.
        /// 1 = castrado, 0 = não castrado
        /// </summary>
        
        [Required(ErrorMessage = "Indique se o pet está castrado")]        
        public int StCastrado { get; set; }

        //FK para Tutor

        [Required(ErrorMessage = "TutorId é obrigatório.")]
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }

        //Um pet pode ter várias consultas
        public ICollection<Consulta>? Consultas { get; set; }
    }
}
