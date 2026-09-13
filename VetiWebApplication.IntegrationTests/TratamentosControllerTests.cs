using System.Net;
using System.Net.Http.Json;

// Testa os endpoints de TratamentosController,
// simulando requisições reais através do WebApplicationFactory.
namespace VetiWebApplication.IntegrationTests
{
    // Compartilha a mesma instância da API de teste
    // (e os mesmos dados mockados) com
    // as demais classes.
    [Collection("Veti API")]
    public class TratamentosControllerTests
    {
        // Cliente HTTP usado para simular requisições reais à API durante os testes.
        private readonly HttpClient dbClient;

        public TratamentosControllerTests(CustomWebApplicationFactory factory)
        {
            // O xUnit injeta aqui a instância compartilhada da fábrica de testes
            // (definida pela Collection Fixture "Veti API"). 
            dbClient = factory.CreateClient();
        }

        // Método auxiliar: cria um Tutor e um Pet, devolvendo o ID do Pet
        // criado, para usar na criação de um Tratamento.
        private async Task<int> CriarPetAsync()
        {
            var novoTutor = new
            {
                NmTutor = "Tutor do Tratamento",
                DsCpf = Guid.NewGuid().ToString("N").Substring(0, 11),
                DsEmail = $"{Guid.NewGuid()}@email.com",
                DsTelefone = "11933333333"
            };
            var respostaTutor = await dbClient.PostAsJsonAsync("/api/tutor", novoTutor);
            var tutorCriado = await respostaTutor.Content.ReadFromJsonAsync<IdResponseDto>();

            var novoPet = new
            {
                NmPet = "Pet do Tratamento",
                DsEspecie = "Cachorro",
                DsRaca = "SRD",
                NrIdade = 3,
                StCastrado = 1,
                TutorId = tutorCriado!.Id
            };
            var respostaPet = await dbClient.PostAsJsonAsync("/api/pet", novoPet);
            var petCriado = await respostaPet.Content.ReadFromJsonAsync<IdResponseDto>();

            return petCriado!.Id;
        }

        [Fact]
        public async Task PostTratamento_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var petId = await CriarPetAsync();

            var novoTratamento = new
            {
                DsDiagnostico = "Infecção bacteriana",
                DtInicio = new DateOnly(2026, 5, 24),
                DtRetornoPrevisto = new DateOnly(2026, 6, 7),
                DsObservacao = "Manter em repouso",
                PetId = petId
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/tratamento", novoTratamento);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostTratamento_ComPetInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var tratamentoInvalido = new
            {
                DsDiagnostico = "Infecção bacteriana",
                DtInicio = new DateOnly(2026, 5, 24),
                PetId = 99999
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/tratamento", tratamentoInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task PostTratamento_SemDiagnostico_DeveRetornarBadRequest()
        {
            // Arrange
            var petId = await CriarPetAsync();

            var tratamentoInvalido = new
            {
                DsDiagnostico = "",
                DtInicio = new DateOnly(2026, 5, 24),
                PetId = petId
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/tratamento", tratamentoInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task PostMedicamentoNoTratamento_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange: cria um Pet, um Tratamento e um Medicamento
            var petId = await CriarPetAsync();

            var novoTratamento = new
            {
                DsDiagnostico = "Infecção bacteriana",
                DtInicio = new DateOnly(2026, 5, 24),
                PetId = petId
            };
            var respostaTratamento = await dbClient.PostAsJsonAsync("/api/tratamento", novoTratamento);
            var tratamentoCriado = await respostaTratamento.Content.ReadFromJsonAsync<IdResponseDto>();

            var novoMedicamento = new
            {
                NmMedicamento = "Amoxicilina",
                DsDosagem = "500mg",
                DsFrequencia = "A cada 8 horas"
            };
            var respostaMedicamento = await dbClient.PostAsJsonAsync("/api/medicamento", novoMedicamento);
            var medicamentoCriado = await respostaMedicamento.Content.ReadFromJsonAsync<IdResponseDto>();

            var vinculo = new
            {
                MedicamentoId = medicamentoCriado!.Id,
                QtMedicamento = 2,
                DsInstrucao = "Tomar após refeição"
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync($"/api/tratamento/{tratamentoCriado!.Id}/medicamento", vinculo);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task GetTratamentos_DeveRetornarOkComListaDeTratamentos()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/tratamento");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }
    }
}