using System.Net;
using System.Net.Http.Json;

// Testa os endpoints do VeterinariosController,
// simulando requisições reais através do WebApplicationFactory.

namespace VetiWebApplication.IntegrationTests
{
    // Compartilha a mesma instância da API de teste
    // (e os mesmos dados mockados) com
    // as demais classes.
    [Collection("Veti API")]
    public class VeterinariosControllerTests
    {
        // Cliente HTTP usado para simular requisições reais à API durante os testes.
        private readonly HttpClient dbClient;

        public VeterinariosControllerTests(CustomWebApplicationFactory factory)
        {
            // O xUnit injeta aqui a instância compartilhada da fábrica de testes
            // (definida pela Collection Fixture "Veti API"). 
            dbClient = factory.CreateClient();
        }

        [Fact]
        public async Task PostVeterinario_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var novoVeterinario = new
            {
                DsEmail = "vet@clinica.com",
                DsPassword = "123456"
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/veterinario", novoVeterinario);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostVeterinario_SemEmail_DeveRetornarBadRequest()
        {
            // Arrange
            var veterinarioInvalido = new
            {
                DsEmail = "",
                DsPassword = "123456"
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/veterinario", veterinarioInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task PostVeterinario_SemSenha_DeveRetornarBadRequest()
        {
            // Arrange
            var veterinarioInvalido = new
            {
                DsEmail = "vet2@clinica.com",
                DsPassword = ""
            };

            // Act
            var resposta = await dbClient.PostAsJsonAsync("/api/veterinario", veterinarioInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task GetVeterinarios_DeveRetornarOkComListaDeVeterinarios()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/veterinario");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task GetVeterinarioById_ComIdInexistente_DeveRetornarNotFound()
        {
            // Act
            var resposta = await dbClient.GetAsync("/api/veterinario/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }
    }
}