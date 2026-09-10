namespace VetiWebApplication.Models.Requests
{
    public class PetRequest
    {
        public string NmPet { get; set; }
        public string DsEspecie { get; set; }
        public string DsRaca { get; set; }
        public int NrIdade { get; set; }
        public int StCastrado { get; set; }
        public int TutorId { get; set; }
    }
}