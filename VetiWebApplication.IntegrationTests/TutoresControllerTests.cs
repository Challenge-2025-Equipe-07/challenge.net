using System.Net;
using System.Net.Http.Json;

// Testa os endpoints do TutoresController,
// simulando requisições reais através do WebApplicationFactory.
namespace VetiWebApplication.IntegrationTests
{
    // Compartilha a mesma instância da API de teste
    // (e os mesmos dados mockados) com
    // as demais classes.
    [Collection("Veti API")]
    public class TutoresControllerTests
    {
        // Cliente HTTP usado para simular requisições reais à API durante os testes.
        private readonly HttpClient dbClient;

        public TutoresControllerTests(CustomWebApplicationFactory factory)
        {
            // O xUnit injeta aqui a instância compartilhada da fábrica de testes
            // (definida pela Collection Fixture "Veti API").
            dbClient = factory.CreateClient();
        }

        [Fact]
        public async Task PostTutor_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var novoTutor = new
            {
                NmTutor = "Laura Lopes",
                DsCpf = "11111111111",
                DsEmail = "laura@email.com",
                DsTelefone = "11999999999"
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/tutor", novoTutor);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task GetTutores_DeveRetornarOkComListaDeTutores()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/tutor");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task PostTutor_SemNome_DeveRetornarBadRequest()
        {
            // Arrange
            var tutorInvalido = new
            {
                NmTutor = "",
                DsCpf = "22222222222",
                DsEmail = "sem-nome@email.com",
                DsTelefone = "11888888888"
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/tutor", tutorInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task GetTutorById_ComIdInexistente_DeveRetornarNotFound()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/tutor/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }
    }
}