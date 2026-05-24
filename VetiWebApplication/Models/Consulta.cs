using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace VetiWebApplication.Models
{
    public class Consulta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Data da consulta é obrigatória.")]
        public DateOnly DtConsulta { get; set; }

        [Required(ErrorMessage = "Tipo de evento é obrigatório.")]
        public string TpEvento { get; set; }  
        public int Notificar { get; set; }

        //FK para Pet

        [Required(ErrorMessage = "Id do pet é obrigatório.")]
        public int PetId { get; set; }
        public Pet Pet { get; set; }

        //FK para Veterinario

        [Required(ErrorMessage = "Id do veterinário é obrigatório.")]
        public int VeterinarioId { get; set; }
        public Veterinario Veterinario { get; set; }

        //Uma consulta pode ter vários exames
        public ICollection<Exame> Exames { get; set; }
    }
}
