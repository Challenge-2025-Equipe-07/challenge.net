using System.Net;
using System.Net.Http.Json;

// Testa os endpoints do ExameMedicamentosController,
// simulando requisições reais através do WebApplicationFactory.
namespace VetiWebApplication.IntegrationTests
{
    // Compartilha a mesma instância da API de teste
    // (e os mesmos dados mockados) com
    // as demais classes.
    [Collection("Veti API")]
    public class ExameMedicamentosControllerTests
    {
        // Cliente HTTP usado para simular requisições reais à API durante os testes.
        private readonly HttpClient dbClient;

        public ExameMedicamentosControllerTests(CustomWebApplicationFactory factory)
        {
            // O xUnit injeta aqui a instância compartilhada da fábrica de testes
            // (definida pela Collection Fixture "Veti API").
            dbClient = factory.CreateClient();
        }

        // Método auxiliar: cria a cadeia completa necessária (Tutor -> Pet ->
        // Veterinário -> Consulta -> Exame, e um Medicamento à parte),
        // devolvendo os IDs do Exame e do Medicamento criados.
        private async Task<(int exameId, int medicamentoId)> CriarExameEMedicamentoAsync()
        {
            var novoTutor = new
            {
                NmTutor = "Tutor ExameMedicamento",
                DsCpf = Guid.NewGuid().ToString("N").Substring(0, 11),
                DsEmail = $"{Guid.NewGuid()}@email.com",
                DsTelefone = "11944444444"
            };
            var respostaTutor = await dbClient.PostAsJsonAsync("/api/tutor", novoTutor);
            var tutorCriado = await respostaTutor.Content.ReadFromJsonAsync<IdResponseDto>();

            var novoPet = new
            {
                NmPet = "Pet ExameMedicamento",
                DsEspecie = "Cachorro",
                DsRaca = "SRD",
                NrIdade = 5,
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

            var novoExame = new
            {
                DsDocumento = "raiox.pdf",
                DtRealizacao = new DateTime(2026, 5, 20),
                DsDiagnostico = "Fratura leve",
                ConsultaId = consultaCriada!.Id
            };
            var respostaExame = await dbClient.PostAsJsonAsync("/api/exames", novoExame);
            var exameCriado = await respostaExame.Content.ReadFromJsonAsync<IdResponseDto>();

            var novoMedicamento = new
            {
                NmMedicamento = "Dipirona",
                DsDosagem = "1 comprimido",
                DsFrequencia = "A cada 6 horas"
            };
            var respostaMedicamento = await dbClient.PostAsJsonAsync("/api/medicamento", novoMedicamento);
            var medicamentoCriado = await respostaMedicamento.Content.ReadFromJsonAsync<IdResponseDto>();

            return (exameCriado!.Id, medicamentoCriado!.Id);
        }

        [Fact]
        public async Task PostExameMedicamento_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var (exameId, medicamentoId) = await CriarExameEMedicamentoAsync();

            var novoVinculo = new
            {
                ExameId = exameId,
                MedicamentoId = medicamentoId,
                QtMedicamento = 2
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/exame-medicamento", novoVinculo);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostExameMedicamento_ComVinculoDuplicado_DeveRetornarBadRequest()
        {
            // Arrange: cria o vínculo uma vez...
            var (exameId, medicamentoId) = await CriarExameEMedicamentoAsync();

            var vinculo = new
            {
                ExameId = exameId,
                MedicamentoId = medicamentoId,
                QtMedicamento = 2
            };
            await dbClient.PostAsJsonAsync("/api/exame-medicamento", vinculo);

            // Act: ...e tenta criar o mesmo vínculo de novo
            var resposta = await dbClient.PostAsJsonAsync("/api/exame-medicamento", vinculo);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task PostExameMedicamento_ComExameInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var (_, medicamentoId) = await CriarExameEMedicamentoAsync();

            var vinculoInvalido = new
            {
                ExameId = 99999,
                MedicamentoId = medicamentoId,
                QtMedicamento = 2
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/exame-medicamento", vinculoInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task GetExameMedicamentos_DeveRetornarOkComLista()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/exame-medicamento");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }
    }
}