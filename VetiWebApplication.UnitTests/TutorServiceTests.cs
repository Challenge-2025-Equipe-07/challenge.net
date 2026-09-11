using Microsoft.Extensions.Logging;
using Moq;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;


namespace VetiWebApplication.UnitTests
{
    // Testes unitários da camada de regra de negócio (Service) do Tutor.
    public class TutorServiceTests
    {
        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveCriarTutorComSucesso()
        {
            // Arrange
            var repositorioMock = new Mock<ITutorRepository>();
            var loggerMock = new Mock<ILogger<TutorService>>();

            repositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Tutor>()))
                .ReturnsAsync((Tutor t) =>
                {
                    t.Id = 1;
                    return t;
                });

            var service = new TutorService(repositorioMock.Object, loggerMock.Object);

            var request = new TutorRequest
            {
                NmTutor = "Laura Lopes",
                DsCpf = "12345678901",
                DsEmail = "laura@email.com",
                DsTelefone = "11999999999"
            };

            // Act
            var resultado = await service.CriarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Laura Lopes", resultado.NmTutor);
            Assert.Equal(1, resultado.Id);

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tutor>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_SemNome_DeveLancarExcecao()
        {
            // Arrange
            var repositorioMock = new Mock<ITutorRepository>();
            var loggerMock = new Mock<ILogger<TutorService>>();
            var service = new TutorService(repositorioMock.Object, loggerMock.Object);

            var request = new TutorRequest
            {
                NmTutor = "",
                DsCpf = "12345678901",
                DsEmail = "laura@email.com",
                DsTelefone = "11999999999"
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tutor>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_SemCpf_DeveLancarExcecao()
        {
            // Arrange
            var repositorioMock = new Mock<ITutorRepository>();
            var loggerMock = new Mock<ILogger<TutorService>>();
            var service = new TutorService(repositorioMock.Object, loggerMock.Object);

            var request = new TutorRequest
            {
                NmTutor = "Laura Lopes",
                DsCpf = "",
                DsEmail = "laura@email.com",
                DsTelefone = "11999999999"
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tutor>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_SemTelefone_DeveLancarExcecao()
        {
            // Arrange
            var repositorioMock = new Mock<ITutorRepository>();
            var loggerMock = new Mock<ILogger<TutorService>>();
            var service = new TutorService(repositorioMock.Object, loggerMock.Object);

            var request = new TutorRequest
            {
                NmTutor = "Laura Lopes",
                DsCpf = "12345678901",
                DsEmail = "laura@email.com",
                DsTelefone = ""
            };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Tutor>()), Times.Never);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarTutor()
        {
            // Arrange
            var tutorExistente = new Tutor { Id = 1, NmTutor = "Laura Lopes", DsCpf = "12345678901", DsTelefone = "11999999999" };

            var repositorioMock = new Mock<ITutorRepository>();
            repositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(tutorExistente);

            var loggerMock = new Mock<ILogger<TutorService>>();
            var service = new TutorService(repositorioMock.Object, loggerMock.Object);

            // Act
            var resultado = await service.ObterPorIdAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Laura Lopes", resultado.NmTutor);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNulo()
        {
            // Arrange
            var repositorioMock = new Mock<ITutorRepository>();
            repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tutor?)null);

            var loggerMock = new Mock<ILogger<TutorService>>();
            var service = new TutorService(repositorioMock.Object, loggerMock.Object);

            // Act
            var resultado = await service.ObterPorIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var repositorioMock = new Mock<ITutorRepository>();
            repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tutor?)null);

            var loggerMock = new Mock<ILogger<TutorService>>();
            var service = new TutorService(repositorioMock.Object, loggerMock.Object);

            var request = new TutorRequest { NmTutor = "Novo Nome", DsCpf = "00000000000", DsTelefone = "11888888888" };

            // Act
            var resultado = await service.AtualizarAsync(999, request);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var repositorioMock = new Mock<ITutorRepository>();
            repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tutor?)null);

            var loggerMock = new Mock<ILogger<TutorService>>();
            var service = new TutorService(repositorioMock.Object, loggerMock.Object);

            // Act
            var resultado = await service.RemoverAsync(999);

            // Assert
            Assert.False(resultado);
        }
    }
}