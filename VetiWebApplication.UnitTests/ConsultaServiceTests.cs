using Microsoft.Extensions.Logging;
using Moq;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.UnitTests
{
    // Testes unitários da camada de regra de negócio (Service) da Consulta.
    public class ConsultaServiceTests
    {

        // Método auxiliar para criar o Service com os três repositórios mockados,
        // evitando repetir essa configuração em cada teste.
        private static ConsultaService CriarService(
            out Mock<IConsultaRepository> consultaRepositorioMock,
            out Mock<IPetRepository> petRepositorioMock,
            out Mock<IVeterinarioRepository> veterinarioRepositorioMock)
        {
            consultaRepositorioMock = new Mock<IConsultaRepository>();
            petRepositorioMock = new Mock<IPetRepository>();
            veterinarioRepositorioMock = new Mock<IVeterinarioRepository>();
            var loggerMock = new Mock<ILogger<ConsultaService>>();

            return new ConsultaService(
                consultaRepositorioMock.Object,
                petRepositorioMock.Object,
                veterinarioRepositorioMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveCriarConsultaComSucesso()
        {
            // Arrange
            var service = CriarService(out var consultaRepositorioMock, out var petRepositorioMock, out var veterinarioRepositorioMock);

            petRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Pet { Id = 1, NmPet = "Sauro" });
            veterinarioRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Veterinario { Id = 1, DsEmail = "vet@clinica.com" });

            consultaRepositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Consulta>()))
                .ReturnsAsync((Consulta c) =>
                {
                    c.Id = 1;
                    return c;
                });

            var request = new ConsultaRequest
            {
                DtConsulta = new DateOnly(2026, 5, 20),
                TpEvento = "Consulta de rotina",
                Notificar = 1,
                PetId = 1,
                VeterinarioId = 1
            };

            // Act
            var resultado = await service.CriarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Consulta de rotina", resultado.TpEvento);
            Assert.Equal(1, resultado.Id);

            consultaRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Consulta>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_SemTipoEvento_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var consultaRepositorioMock, out var petRepositorioMock, out var veterinarioRepositorioMock);

            var request = new ConsultaRequest { TpEvento = "", PetId = 1, VeterinarioId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            consultaRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Consulta>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_ComPetInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var consultaRepositorioMock, out var petRepositorioMock, out var veterinarioRepositorioMock);

            petRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Pet?)null);

            var request = new ConsultaRequest { TpEvento = "Consulta de rotina", PetId = 999, VeterinarioId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CriarAsync(request));

            consultaRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Consulta>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_ComVeterinarioInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var consultaRepositorioMock, out var petRepositorioMock, out var veterinarioRepositorioMock);

            petRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Pet { Id = 1, NmPet = "Sauro" });
            veterinarioRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Veterinario?)null);

            var request = new ConsultaRequest { TpEvento = "Consulta de rotina", PetId = 1, VeterinarioId = 999 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CriarAsync(request));

            consultaRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Consulta>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComConsultaInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var consultaRepositorioMock, out var petRepositorioMock, out var veterinarioRepositorioMock);

            consultaRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Consulta?)null);

            var request = new ConsultaRequest { TpEvento = "Retorno", PetId = 1, VeterinarioId = 1 };

            // Act
            var (sucesso, vetNaoEncontrado) = await service.AtualizarAsync(999, request);

            // Assert
            Assert.False(sucesso);
            Assert.False(vetNaoEncontrado);
        }

        [Fact]
        public async Task AtualizarAsync_ComVeterinarioInexistente_DeveRetornarVeterinarioNaoEncontrado()
        {
            // Arrange
            var service = CriarService(out var consultaRepositorioMock, out var petRepositorioMock, out var veterinarioRepositorioMock);

            var consultaExistente = new Consulta { Id = 1, TpEvento = "Retorno" };
            consultaRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(consultaExistente);
            veterinarioRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Veterinario?)null);

            var request = new ConsultaRequest { TpEvento = "Retorno", PetId = 1, VeterinarioId = 999 };

            // Act
            var (sucesso, vetNaoEncontrado) = await service.AtualizarAsync(1, request);

            // Assert
            Assert.False(sucesso);
            Assert.True(vetNaoEncontrado);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var consultaRepositorioMock, out var petRepositorioMock, out var veterinarioRepositorioMock);

            consultaRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Consulta?)null);

            // Act
            var resultado = await service.RemoverAsync(999);

            // Assert
            Assert.False(resultado);
        }
    }
}