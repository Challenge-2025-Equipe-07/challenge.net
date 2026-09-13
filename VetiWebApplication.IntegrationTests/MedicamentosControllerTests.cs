using System.Net;
using System.Net.Http.Json;

// Testa os endpoints do MedicamentosController,
// simulando requisições reais através do WebApplicationFactory.
namespace VetiWebApplication.IntegrationTests
{

    // Compartilha a mesma instância da API de teste
    // (e os mesmos dados mockados) com
    // as demais classes.
    [Collection("Veti API")]
    public class MedicamentosControllerTests
    {
        // Cliente HTTP usado para simular requisições reais à API durante os testes.
        private readonly HttpClient dbClient;

        public MedicamentosControllerTests(CustomWebApplicationFactory factory)
        {
            // O xUnit injeta aqui a instância compartilhada da fábrica de testes
            // (definida pela Collection Fixture "Veti API").
            dbClient = factory.CreateClient();
        }

        [Fact]
        public async Task PostMedicamento_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var novoMedicamento = new
            {
                NmMedicamento = "Amoxicilina",
                DsDosagem = "500mg",
                DsFrequencia = "A cada 8 horas"
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/medicamento", novoMedicamento);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostMedicamento_SemNome_DeveRetornarBadRequest()
        {
            // Arrange
            var medicamentoInvalido = new
            {
                NmMedicamento = "",
                DsDosagem = "500mg",
                DsFrequencia = "A cada 8 horas"
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/medicamento", medicamentoInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task PostMedicamento_SemDosagem_DeveRetornarBadRequest()
        {
            // Arrange
            var medicamentoInvalido = new
            {
                NmMedicamento = "Amoxicilina",
                DsDosagem = "",
                DsFrequencia = "A cada 8 horas"
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/medicamento", medicamentoInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task PostMedicamento_SemFrequencia_DeveRetornarBadRequest()
        {
            // Arrange
            var medicamentoInvalido = new
            {
                NmMedicamento = "Amoxicilina",
                DsDosagem = "500mg",
                DsFrequencia = ""
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/medicamento", medicamentoInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task GetMedicamentos_DeveRetornarOkComListaDeMedicamentos()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/medicamento");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task GetMedicamentoById_ComIdInexistente_DeveRetornarNotFound()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/medicamento/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }
    }
}