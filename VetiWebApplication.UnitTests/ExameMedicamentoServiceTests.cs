using Microsoft.Extensions.Logging;
using Moq;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.UnitTests
{
    // Testes unitários da camada de regra de negócio (Service) do
    // Medicamento vinculado a um exame.
    public class ExameMedicamentoServiceTests
    {
        // Método auxiliar para criar o Service com os três repositórios mockados,
        // evitando repetir essa configuração em cada teste.
        private static ExameMedicamentoService CriarService(
            out Mock<IExameMedicamentoRepository> exameMedicamentoRepositorioMock,
            out Mock<IExameRepository> exameRepositorioMock,
            out Mock<IMedicamentoRepository> medicamentoRepositorioMock)
        {
            exameMedicamentoRepositorioMock = new Mock<IExameMedicamentoRepository>();
            exameRepositorioMock = new Mock<IExameRepository>();
            medicamentoRepositorioMock = new Mock<IMedicamentoRepository>();
            var loggerMock = new Mock<ILogger<ExameMedicamentoService>>();

            return new ExameMedicamentoService(
                exameMedicamentoRepositorioMock.Object,
                exameRepositorioMock.Object,
                medicamentoRepositorioMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveVincularComSucesso()
        {
            // Arrange
            var service = CriarService(out var exameMedicamentoRepositorioMock, out var exameRepositorioMock, out var medicamentoRepositorioMock);

            exameRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Exame { Id = 1, DsDocumento = "hemograma.pdf" });
            medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Medicamento { Id = 1, NmMedicamento = "Amoxicilina" });
            exameMedicamentoRepositorioMock.Setup(r => r.ExisteAsync(1, 1)).ReturnsAsync(false);

            exameMedicamentoRepositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<ExameMedicamento>()))
                .ReturnsAsync((ExameMedicamento em) => em);

            var request = new ExameMedicamentoRequest { ExameId = 1, MedicamentoId = 1, QtMedicamento = 2 };

            // Act
            var resultado = await service.CriarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.ExameId);
            Assert.Equal(1, resultado.MedicamentoId);

            exameMedicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<ExameMedicamento>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_ComExameInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var exameMedicamentoRepositorioMock, out var exameRepositorioMock, out var medicamentoRepositorioMock);

            exameRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Exame?)null);

            var request = new ExameMedicamentoRequest { ExameId = 999, MedicamentoId = 1, QtMedicamento = 2 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CriarAsync(request));

            exameMedicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<ExameMedicamento>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_ComMedicamentoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var exameMedicamentoRepositorioMock, out var exameRepositorioMock, out var medicamentoRepositorioMock);

            exameRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Exame { Id = 1, DsDocumento = "hemograma.pdf" });
            medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Medicamento?)null);

            var request = new ExameMedicamentoRequest { ExameId = 1, MedicamentoId = 999, QtMedicamento = 2 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CriarAsync(request));

            exameMedicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<ExameMedicamento>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_ComRelacaoJaExistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var exameMedicamentoRepositorioMock, out var exameRepositorioMock, out var medicamentoRepositorioMock);

            exameRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Exame { Id = 1, DsDocumento = "hemograma.pdf" });
            medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Medicamento { Id = 1, NmMedicamento = "Amoxicilina" });
            exameMedicamentoRepositorioMock.Setup(r => r.ExisteAsync(1, 1)).ReturnsAsync(true);

            var request = new ExameMedicamentoRequest { ExameId = 1, MedicamentoId = 1, QtMedicamento = 2 };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarAsync(request));

            exameMedicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<ExameMedicamento>()), Times.Never);
        }

        [Fact]
        public async Task ObterPorExameAsync_ComExameInexistente_DeveRetornarExameNaoExiste()
        {
            // Arrange
            var service = CriarService(out var exameMedicamentoRepositorioMock, out var exameRepositorioMock, out var medicamentoRepositorioMock);

            exameRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Exame?)null);

            // Act
            var (exameExiste, lista) = await service.ObterPorExameAsync(999);

            // Assert
            Assert.False(exameExiste);
            Assert.Empty(lista);
        }

        [Fact]
        public async Task ObterPorMedicamentoAsync_ComMedicamentoInexistente_DeveRetornarMedicamentoNaoExiste()
        {
            // Arrange
            var service = CriarService(out var exameMedicamentoRepositorioMock, out var exameRepositorioMock, out var medicamentoRepositorioMock);

            medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Medicamento?)null);

            // Act
            var (medicamentoExiste, lista) = await service.ObterPorMedicamentoAsync(999);

            // Assert
            Assert.False(medicamentoExiste);
            Assert.Empty(lista);
        }

        [Fact]
        public async Task RemoverAsync_ComRelacaoInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var exameMedicamentoRepositorioMock, out var exameRepositorioMock, out var medicamentoRepositorioMock);

            exameMedicamentoRepositorioMock
                .Setup(r => r.ObterPorChaveAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((ExameMedicamento?)null);

            // Act
            var resultado = await service.RemoverAsync(999, 999);

            // Assert
            Assert.False(resultado);
        }
    }
}