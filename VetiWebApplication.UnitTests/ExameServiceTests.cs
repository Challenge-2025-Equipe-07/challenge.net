using Microsoft.Extensions.Logging;
using Moq;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;


namespace VetiWebApplication.UnitTests
{
    // Testes unitários da camada de regra de negócio (Service) do Exame.
    public class ExameServiceTests
    {
        // Método auxiliar para criar o Service com os três repositórios mockados,
        // evitando repetir essa configuração em cada teste.
        private static ExameService CriarService(
            out Mock<IExameRepository> exameRepositorioMock,
            out Mock<IConsultaRepository> consultaRepositorioMock,
            out Mock<ITutorRepository> tutorRepositorioMock)
        {
            exameRepositorioMock = new Mock<IExameRepository>();
            consultaRepositorioMock = new Mock<IConsultaRepository>();
            tutorRepositorioMock = new Mock<ITutorRepository>();
            var loggerMock = new Mock<ILogger<ExameService>>();

            return new ExameService(
                exameRepositorioMock.Object,
                consultaRepositorioMock.Object,
                tutorRepositorioMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveCriarExameComSucesso()
        {
            // Arrange
            var service = CriarService(out var exameRepositorioMock, out var consultaRepositorioMock, out var tutorRepositorioMock);

            consultaRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Consulta { Id = 1, TpEvento = "Consulta de rotina" });

            exameRepositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Exame>()))
                .ReturnsAsync((Exame e) =>
                {
                    e.Id = 1;
                    return e;
                });

            var request = new ExameRequest
            {
                DsDocumento = "hemograma.pdf",
                DtRealizacao = new DateTime(2026, 5, 20),
                DsDiagnostico = "Anemia leve",
                ConsultaId = 1
            };

            // Act
            var resultado = await service.CriarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("hemograma.pdf", resultado.DsDocumento);
            Assert.Equal(1, resultado.Id);

            exameRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Exame>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_SemDocumento_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var exameRepositorioMock, out var consultaRepositorioMock, out var tutorRepositorioMock);

            var request = new ExameRequest { DsDocumento = "", DsDiagnostico = "Anemia leve", ConsultaId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            exameRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Exame>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_SemDiagnostico_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var exameRepositorioMock, out var consultaRepositorioMock, out var tutorRepositorioMock);

            var request = new ExameRequest { DsDocumento = "hemograma.pdf", DsDiagnostico = "", ConsultaId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            exameRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Exame>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_ComConsultaInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var exameRepositorioMock, out var consultaRepositorioMock, out var tutorRepositorioMock);

            consultaRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Consulta?)null);

            var request = new ExameRequest { DsDocumento = "hemograma.pdf", DsDiagnostico = "Anemia leve", ConsultaId = 999 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CriarAsync(request));

            exameRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Exame>()), Times.Never);
        }

        [Fact]
        public async Task ObterPorTutorAsync_ComTutorInexistente_DeveRetornarTutorNaoExiste()
        {
            // Arrange
            var service = CriarService(out var exameRepositorioMock, out var consultaRepositorioMock, out var tutorRepositorioMock);

            tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tutor?)null);

            // Act
            var (tutorExiste, exames) = await service.ObterPorTutorAsync(999);

            // Assert
            Assert.False(tutorExiste);
            Assert.Empty(exames);
        }

        [Fact]
        public async Task ObterPorTutorAsync_ComTutorExistente_DeveRetornarExamesDoTutor()
        {
            // Arrange
            var service = CriarService(out var exameRepositorioMock, out var consultaRepositorioMock, out var tutorRepositorioMock);

            var tutor = new Tutor { Id = 1, NmTutor = "Laura Lopes" };
            tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(tutor);

            var pet = new Pet { Id = 1, NmPet = "Sauro", TutorId = 1 };
            var consulta = new Consulta { Id = 1, Pet = pet };
            var exameDoTutor = new Exame { Id = 1, DsDocumento = "hemograma.pdf", Consulta = consulta };

            exameRepositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(new List<Exame> { exameDoTutor });

            // Act
            var (tutorExiste, exames) = await service.ObterPorTutorAsync(1);

            // Assert
            Assert.True(tutorExiste);
            Assert.Single(exames);
            Assert.Equal("hemograma.pdf", exames.First().DsDocumento);
        }

        [Fact]
        public async Task AtualizarAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var exameRepositorioMock, out var consultaRepositorioMock, out var tutorRepositorioMock);

            exameRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Exame?)null);

            var request = new ExameRequest { DsDocumento = "hemograma.pdf", DsDiagnostico = "Anemia leve", ConsultaId = 1 };

            // Act
            var resultado = await service.AtualizarAsync(999, request);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var exameRepositorioMock, out var consultaRepositorioMock, out var tutorRepositorioMock);

            exameRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Exame?)null);

            // Act
            var resultado = await service.RemoverAsync(999);

            // Assert
            Assert.False(resultado);
        }
    }
}