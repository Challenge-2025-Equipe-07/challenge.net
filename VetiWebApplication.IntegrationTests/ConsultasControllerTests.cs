using System.Net;
using System.Net.Http.Json;


// Testa os endpoints de ConsultasController,
// simulando requisições reais através do WebApplicationFactory.
namespace VetiWebApplication.IntegrationTests
{
    // Compartilha a mesma instância da API de teste
    // (e os mesmos dados mockados) com
    // as demais classes.
    [Collection("Veti API")]
    public class ConsultasControllerTests
    {
        // Cliente HTTP usado para simular requisições reais à API durante os testes.
        private readonly HttpClient dbClient;

        public ConsultasControllerTests(CustomWebApplicationFactory factory)
        {
            // O xUnit injeta aqui a instância compartilhada da fábrica de testes
            // (definida pela Collection Fixture "Veti API").
            dbClient = factory.CreateClient();
        }

        // Método auxiliar: cria um Tutor, um Pet e um Veterinário válidos,
        // e devolve os IDs gerados, para usar na criação de uma Consulta.
        private async Task<(int petId, int veterinarioId)> CriarPetEVeterinarioAsync()
        {
            var novoTutor = new
            {
                NmTutor = "Tutor da Consulta",
                DsCpf = Guid.NewGuid().ToString("N").Substring(0, 11),
                DsEmail = $"{Guid.NewGuid()}@email.com",
                DsTelefone = "11966666666"
            };
            var respostaTutor = await dbClient.PostAsJsonAsync("/api/tutor", novoTutor);
            var tutorCriado = await respostaTutor.Content.ReadFromJsonAsync<IdResponseDto>();

            var novoPet = new
            {
                NmPet = "Pet da Consulta",
                DsEspecie = "Cachorro",
                DsRaca = "SRD",
                NrIdade = 2,
                StCastrado = 1,
                TutorId = tutorCriado!.Id
            };
            var respostaPet = await dbClient.PostAsJsonAsync("/api/pet", novoPet);
            var petCriado = await respostaPet.Content.ReadFromJsonAsync<IdResponseDto>();

            var novoVeterinario = new
            {
                DsEmail = $"{Guid.NewGuid()}@clinica.com",
                DsPassword = "123456"
            };
            var respostaVet = await dbClient.PostAsJsonAsync("/api/veterinario", novoVeterinario);
            var vetCriado = await respostaVet.Content.ReadFromJsonAsync<IdResponseDto>();

            return (petCriado!.Id, vetCriado!.Id);
        }

        [Fact]
        public async Task PostConsulta_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var (petId, veterinarioId) = await CriarPetEVeterinarioAsync();

            var novaConsulta = new
            {
                DtConsulta = new DateOnly(2026, 5, 20),
                TpEvento = "Consulta de rotina",
                Notificar = 1,
                PetId = petId,
                VeterinarioId = veterinarioId
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/consulta", novaConsulta);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostConsulta_ComPetInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var (_, veterinarioId) = await CriarPetEVeterinarioAsync();

            var consultaInvalida = new
            {
                DtConsulta = new DateOnly(2026, 5, 20),
                TpEvento = "Consulta de rotina",
                Notificar = 1,
                PetId = 99999,
                VeterinarioId = veterinarioId
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/consulta", consultaInvalida);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task PostConsulta_SemTipoEvento_DeveRetornarBadRequest()
        {
            // Arrange
            var (petId, veterinarioId) = await CriarPetEVeterinarioAsync();

            var consultaInvalida = new
            {
                DtConsulta = new DateOnly(2026, 5, 20),
                TpEvento = "",
                Notificar = 1,
                PetId = petId,
                VeterinarioId = veterinarioId
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/consulta", consultaInvalida);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task GetConsultas_DeveRetornarOkComListaDeConsultas()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/consulta");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }
    }

    // DTO simples, usado só nos testes, para ler o "Id" de qualquer entidade
    // criada na resposta JSON de um POST.
    internal class IdResponseDto
    {
        public int Id { get; set; }
    }
}