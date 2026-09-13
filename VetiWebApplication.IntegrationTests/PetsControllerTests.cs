using System.Net;
using System.Net.Http.Json;

// Testa os endpoints do PetsController,
// simulando requisições reais através do WebApplicationFactory.

namespace VetiWebApplication.IntegrationTests
{
    // Compartilha a mesma instância da API de teste
    // (e os mesmos dados mockados) com
    // as demais classes.
    [Collection("Veti API")]
    public class PetsControllerTests
    {
        // Cliente HTTP usado para simular requisições reais à API durante os testes.
        private readonly HttpClient dbClient;

        public PetsControllerTests(CustomWebApplicationFactory factory)
        {
            // O xUnit injeta aqui a instância compartilhada da fábrica de testes
            // (definida pela Collection Fixture "Veti API").
            dbClient = factory.CreateClient();
        }

        [Fact]
        public async Task PostPet_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange: garante que existe um tutor para vincular o pet
            var novoTutor = new
            {
                NmTutor = "Tutor do Pet",
                DsCpf = "33333333333",
                DsEmail = "tutor.pet@email.com",
                DsTelefone = "11977777777"
            };
            var respostaTutor = await dbClient.PostAsJsonAsync("/api/tutor", novoTutor);
            var tutorCriado = await respostaTutor.Content.ReadFromJsonAsync<IdResponseDto>();

            var novoPet = new
            {
                NmPet = "Sauro",
                DsEspecie = "Gato",
                DsRaca = "SRD",
                NrIdade = 6,
                StCastrado = 1,
                TutorId = tutorCriado!.Id
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/pet", novoPet);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostPet_ComTutorInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var petInvalido = new
            {
                NmPet = "Rex",
                DsEspecie = "Cachorro",
                DsRaca = "SRD",
                NrIdade = 3,
                StCastrado = 0,
                TutorId = 99999
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/pet", petInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task PostPet_SemNome_DeveRetornarBadRequest()
        {
            // Arrange
            var petInvalido = new
            {
                NmPet = "",
                DsEspecie = "Cachorro",
                DsRaca = "SRD",
                NrIdade = 3,
                StCastrado = 0,
                TutorId = 1
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/pet", petInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task GetPets_DeveRetornarOkComListaDePets()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/pet");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }
    }
}