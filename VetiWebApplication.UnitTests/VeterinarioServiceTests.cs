using Microsoft.Extensions.Logging;
using Moq;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.UnitTests
{
    // Testes unitários da camada de regra de negócio (Service) do Veterinário.
    public class VeterinarioServiceTests
    {
        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveCriarVeterinarioComSucesso()
        {
            // Arrange
            var repositorioMock = new Mock<IVeterinarioRepository>();
            var loggerMock = new Mock<ILogger<VeterinarioService>>();

            repositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Veterinario>()))
                .ReturnsAsync((Veterinario v) =>
                {
                    v.Id = 1;
                    return v;
                });

            var service = new VeterinarioService(repositorioMock.Object, loggerMock.Object);

            var request = new VeterinarioRequest
            {
                DsEmail = "vet@clinica.com",
                DsPassword = "123456"
            };

            // Act
            var resultado = await service.CriarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("vet@clinica.com", resultado.DsEmail);
            Assert.Equal(1, resultado.Id);

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Veterinario>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_SemEmail_DeveLancarExcecao()
        {
            // Arrange
            var repositorioMock = new Mock<IVeterinarioRepository>();
            var loggerMock = new Mock<ILogger<VeterinarioService>>();
            var service = new VeterinarioService(repositorioMock.Object, loggerMock.Object);

            var request = new VeterinarioRequest { DsEmail = "", DsPassword = "123456" };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Veterinario>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_SemSenha_DeveLancarExcecao()
        {
            // Arrange
            var repositorioMock = new Mock<IVeterinarioRepository>();
            var loggerMock = new Mock<ILogger<VeterinarioService>>();
            var service = new VeterinarioService(repositorioMock.Object, loggerMock.Object);

            var request = new VeterinarioRequest { DsEmail = "vet@clinica.com", DsPassword = "" };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Veterinario>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var repositorioMock = new Mock<IVeterinarioRepository>();
            repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Veterinario?)null);

            var loggerMock = new Mock<ILogger<VeterinarioService>>();
            var service = new VeterinarioService(repositorioMock.Object, loggerMock.Object);

            var request = new VeterinarioRequest { DsEmail = "vet@clinica.com", DsPassword = "123456" };

            // Act
            var resultado = await service.AtualizarAsync(999, request);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var repositorioMock = new Mock<IVeterinarioRepository>();
            repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Veterinario?)null);

            var loggerMock = new Mock<ILogger<VeterinarioService>>();
            var service = new VeterinarioService(repositorioMock.Object, loggerMock.Object);

            // Act
            var resultado = await service.RemoverAsync(999);

            // Assert
            Assert.False(resultado);
        }
    }
}