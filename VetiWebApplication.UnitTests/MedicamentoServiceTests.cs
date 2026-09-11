using Microsoft.Extensions.Logging;
using Moq;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.UnitTests
{
    // Testes unitários da camada de regra de negócio (Service) do Medicamento.
    public class MedicamentoServiceTests
    {
        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveCriarMedicamentoComSucesso()
        {
            // Arrange
            var repositorioMock = new Mock<IMedicamentoRepository>();
            var loggerMock = new Mock<ILogger<MedicamentoService>>();

            repositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Medicamento>()))
                .ReturnsAsync((Medicamento m) =>
                {
                    m.Id = 1;
                    return m;
                });

            var service = new MedicamentoService(repositorioMock.Object, loggerMock.Object);

            var request = new MedicamentoRequest
            {
                NmMedicamento = "Amoxicilina",
                DsDosagem = "500mg",
                DsFrequencia = "A cada 8 horas"
            };

            // Act
            var resultado = await service.CriarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Amoxicilina", resultado.NmMedicamento);
            Assert.Equal(1, resultado.Id);

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Medicamento>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_SemNome_DeveLancarExcecao()
        {
            // Arrange
            var repositorioMock = new Mock<IMedicamentoRepository>();
            var loggerMock = new Mock<ILogger<MedicamentoService>>();
            var service = new MedicamentoService(repositorioMock.Object, loggerMock.Object);

            var request = new MedicamentoRequest { NmMedicamento = "", DsDosagem = "500mg", DsFrequencia = "A cada 8 horas" };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Medicamento>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_SemDosagem_DeveLancarExcecao()
        {
            // Arrange
            var repositorioMock = new Mock<IMedicamentoRepository>();
            var loggerMock = new Mock<ILogger<MedicamentoService>>();
            var service = new MedicamentoService(repositorioMock.Object, loggerMock.Object);

            var request = new MedicamentoRequest { NmMedicamento = "Amoxicilina", DsDosagem = "", DsFrequencia = "A cada 8 horas" };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Medicamento>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_SemFrequencia_DeveLancarExcecao()
        {
            // Arrange
            var repositorioMock = new Mock<IMedicamentoRepository>();
            var loggerMock = new Mock<ILogger<MedicamentoService>>();
            var service = new MedicamentoService(repositorioMock.Object, loggerMock.Object);

            var request = new MedicamentoRequest { NmMedicamento = "Amoxicilina", DsDosagem = "500mg", DsFrequencia = "" };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Medicamento>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var repositorioMock = new Mock<IMedicamentoRepository>();
            repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Medicamento?)null);

            var loggerMock = new Mock<ILogger<MedicamentoService>>();
            var service = new MedicamentoService(repositorioMock.Object, loggerMock.Object);

            var request = new MedicamentoRequest { NmMedicamento = "Amoxicilina", DsDosagem = "500mg", DsFrequencia = "A cada 8 horas" };

            // Act
            var resultado = await service.AtualizarAsync(999, request);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var repositorioMock = new Mock<IMedicamentoRepository>();
            repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Medicamento?)null);

            var loggerMock = new Mock<ILogger<MedicamentoService>>();
            var service = new MedicamentoService(repositorioMock.Object, loggerMock.Object);

            // Act
            var resultado = await service.RemoverAsync(999);

            // Assert
            Assert.False(resultado);
        }
    }
}