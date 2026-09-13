using System.Net;
using System.Net.Http.Json;

// Testa os endpoints do ExamesController,
// simulando requisições reais através do WebApplicationFactory.
namespace VetiWebApplication.IntegrationTests
{
    // Compartilha a mesma instância da API de teste
    // (e os mesmos dados mockados) com
    // as demais classes.
    [Collection("Veti API")]
    public class ExamesControllerTests
    {
        // Cliente HTTP usado para simular requisições reais à API durante os testes.
        private readonly HttpClient dbClient;

        public ExamesControllerTests(CustomWebApplicationFactory factory)
        {
            // O xUnit injeta aqui a instância compartilhada da fábrica de testes
            // (definida pela Collection Fixture "Veti API").
            dbClient = factory.CreateClient();
        }

        // Método auxiliar: cria toda a cadeia necessária (Tutor -> Pet -> Veterinário -> Consulta)
        // e devolve o ID da Consulta gerada, para usar na criação de um Exame.
        private async Task<int> CriarConsultaAsync()
        {
            var novoTutor = new
            {
                NmTutor = "Tutor do Exame",
                DsCpf = Guid.NewGuid().ToString("N").Substring(0, 11),
                DsEmail = $"{Guid.NewGuid()}@email.com",
                DsTelefone = "11955555555"
            };
            var respostaTutor = await dbClient.PostAsJsonAsync("/api/tutor", novoTutor);
            var tutorCriado = await respostaTutor.Content.ReadFromJsonAsync<IdResponseDto>();

            var novoPet = new
            {
                NmPet = "Pet do Exame",
                DsEspecie = "Gato",
                DsRaca = "SRD",
                NrIdade = 4,
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

            var novaConsulta = new
            {
                DtConsulta = new DateOnly(2026, 5, 20),
                TpEvento = "Consulta de rotina",
                Notificar = 1,
                PetId = petCriado!.Id,
                VeterinarioId = vetCriado!.Id
            };
            var respostaConsulta = await dbClient.PostAsJsonAsync("/api/consulta", novaConsulta);
            var consultaCriada = await respostaConsulta.Content.ReadFromJsonAsync<IdResponseDto>();

            return consultaCriada!.Id;
        }

        [Fact]
        public async Task PostExame_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var consultaId = await CriarConsultaAsync();

            var novoExame = new
            {
                DsDocumento = "hemograma.pdf",
                DtRealizacao = new DateTime(2026, 5, 20),
                DsDiagnostico = "Anemia leve",
                ConsultaId = consultaId
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/exames", novoExame);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostExame_ComConsultaInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var exameInvalido = new
            {
                DsDocumento = "hemograma.pdf",
                DtRealizacao = new DateTime(2026, 5, 20),
                DsDiagnostico = "Anemia leve",
                ConsultaId = 99999
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/exames", exameInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task PostExame_SemDocumento_DeveRetornarBadRequest()
        {
            // Arrange
            var consultaId = await CriarConsultaAsync();

            var exameInvalido = new
            {
                DsDocumento = "",
                DtRealizacao = new DateTime(2026, 5, 20),
                DsDiagnostico = "Anemia leve",
                ConsultaId = consultaId
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/exames", exameInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task PostExame_SemDiagnostico_DeveRetornarBadRequest()
        {
            // Arrange
            var consultaId = await CriarConsultaAsync();

            var exameInvalido = new
            {
                DsDocumento = "hemograma.pdf",
                DtRealizacao = new DateTime(2026, 5, 20),
                DsDiagnostico = "",
                ConsultaId = consultaId
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/exames", exameInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task GetExames_DeveRetornarOkComListaDeExames()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/exames");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }
    }
}