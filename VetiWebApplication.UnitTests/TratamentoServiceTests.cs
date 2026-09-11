using Microsoft.Extensions.Logging;
using Moq;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.UnitTests
{
    // Testes unitários da camada de regra de negócio (Service)
    // do Tratamento.
    public class TratamentoServiceTests
    {
        // Método auxiliar para criar o Service com os quatro repositórios mockados,
        // evitando repetir essa configuração em cada teste.
        private static TratamentoService CriarService(
            out Mock<ITratamentoRepository> tratamentoRepositorioMock,
            out Mock<ITratamentoMedicamentoRepository> tratamentoMedicamentoRepositorioMock,
            out Mock<IPetRepository> petRepositorioMock,
            out Mock<IMedicamentoRepository> medicamentoRepositorioMock)
        {
            tratamentoRepositorioMock = new Mock<ITratamentoRepository>();
            tratamentoMedicamentoRepositorioMock = new Mock<ITratamentoMedicamentoRepository>();
            petRepositorioMock = new Mock<IPetRepository>();
            medicamentoRepositorioMock = new Mock<IMedicamentoRepository>();
            var loggerMock = new Mock<ILogger<TratamentoService>>();

            return new TratamentoService(
                tratamentoRepositorioMock.Object,
                tratamentoMedicamentoRepositorioMock.Object,
                petRepositorioMock.Object,
                medicamentoRepositorioMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveCriarTratamentoComSucesso()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            petRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Pet { Id = 1, NmPet = "Sauro" });

            tratamentoRepositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Tratamento>()))
                .ReturnsAsync((Tratamento t) =>
                {
                    t.Id = 1;
                    return t;
                });

            var request = new TratamentoRequest
            {
                DsDiagnostico = "Infecção bacteriana",
                DtInicio = new DateOnly(2026, 5, 24),
                DtRetornoPrevisto = new DateOnly(2026, 6, 7),
                DsObservacao = "Manter em repouso",
                PetId = 1
            };

            // Act
            var resultado = await service.CriarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Infecção bacteriana", resultado.DsDiagnostico);
            Assert.Equal(1, resultado.Id);

            tratamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tratamento>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_SemDiagnostico_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            var request = new TratamentoRequest { DsDiagnostico = "", PetId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            tratamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tratamento>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_ComPetInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            petRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Pet?)null);

            var request = new TratamentoRequest { DsDiagnostico = "Infecção bacteriana", PetId = 999 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CriarAsync(request));

            tratamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tratamento>()), Times.Never);
        }

        [Fact]
        public async Task AdicionarMedicamentoAsync_ComDadosValidos_DeveVincularComSucesso()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            tratamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Tratamento { Id = 1, DsDiagnostico = "Infecção bacteriana" });
            medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Medicamento { Id = 1, NmMedicamento = "Amoxicilina" });
            tratamentoMedicamentoRepositorioMock.Setup(r => r.ExisteAsync(1, 1)).ReturnsAsync(false);

            tratamentoMedicamentoRepositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<TratamentoMedicamento>()))
                .ReturnsAsync((TratamentoMedicamento tm) => tm);

            var request = new TratamentoMedicamentoRequest { MedicamentoId = 1, QtMedicamento = 2, DsInstrucao = "Tomar após refeição" };

            // Act
            var resultado = await service.AdicionarMedicamentoAsync(1, request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.TratamentoId);
            Assert.Equal(1, resultado.MedicamentoId);

            tratamentoMedicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<TratamentoMedicamento>()), Times.Once);
        }

        [Fact]
        public async Task AdicionarMedicamentoAsync_ComTratamentoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            tratamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tratamento?)null);

            var request = new TratamentoMedicamentoRequest { MedicamentoId = 1, QtMedicamento = 2 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AdicionarMedicamentoAsync(999, request));

            tratamentoMedicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<TratamentoMedicamento>()), Times.Never);
        }

        [Fact]
        public async Task AdicionarMedicamentoAsync_ComMedicamentoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            tratamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Tratamento { Id = 1, DsDiagnostico = "Infecção bacteriana" });
            medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Medicamento?)null);

            var request = new TratamentoMedicamentoRequest { MedicamentoId = 999, QtMedicamento = 2 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AdicionarMedicamentoAsync(1, request));

            tratamentoMedicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<TratamentoMedicamento>()), Times.Never);
        }

        [Fact]
        public async Task AdicionarMedicamentoAsync_ComMedicamentoJaVinculado_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            tratamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Tratamento { Id = 1, DsDiagnostico = "Infecção bacteriana" });
            medicamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Medicamento { Id = 1, NmMedicamento = "Amoxicilina" });
            tratamentoMedicamentoRepositorioMock.Setup(r => r.ExisteAsync(1, 1)).ReturnsAsync(true);

            var request = new TratamentoMedicamentoRequest { MedicamentoId = 1, QtMedicamento = 2 };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AdicionarMedicamentoAsync(1, request));

            tratamentoMedicamentoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<TratamentoMedicamento>()), Times.Never);
        }

        [Fact]
        public async Task ObterPorPetAsync_ComPetInexistente_DeveRetornarPetNaoExiste()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            petRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Pet?)null);

            // Act
            var (petExiste, lista) = await service.ObterPorPetAsync(999);

            // Assert
            Assert.False(petExiste);
            Assert.Empty(lista);
        }

        [Fact]
        public async Task AtualizarAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            tratamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tratamento?)null);

            var request = new TratamentoRequest { DsDiagnostico = "Infecção bacteriana", PetId = 1 };

            // Act
            var resultado = await service.AtualizarAsync(999, request);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var tratamentoRepositorioMock, out var tratamentoMedicamentoRepositorioMock, out var petRepositorioMock, out var medicamentoRepositorioMock);

            tratamentoRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tratamento?)null);

            // Act
            var resultado = await service.RemoverAsync(999);

            // Assert
            Assert.False(resultado);
        }
    }
}