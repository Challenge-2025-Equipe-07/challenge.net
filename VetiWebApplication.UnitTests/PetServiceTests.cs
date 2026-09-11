using Microsoft.Extensions.Logging;
using Moq;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.UnitTests
{
    // Testes unitários da camada de regra de negócio (Service) do Pet.
    public class PetServiceTests
    {
        // Método auxiliar para criar o Service com os dois repositórios mockados,
        // evitando repetir essa configuração em cada teste.
        private static PetService CriarService(
            out Mock<IPetRepository> petRepositorioMock,
            out Mock<ITutorRepository> tutorRepositorioMock)
        {
            petRepositorioMock = new Mock<IPetRepository>();
            tutorRepositorioMock = new Mock<ITutorRepository>();
            var loggerMock = new Mock<ILogger<PetService>>();

            return new PetService(petRepositorioMock.Object, tutorRepositorioMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveCriarPetComSucesso()
        {
            // Arrange
            var service = CriarService(out var petRepositorioMock, out var tutorRepositorioMock);

            var tutorExistente = new Tutor { Id = 1, NmTutor = "Laura Lopes" };
            tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(tutorExistente);

            petRepositorioMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Pet>()))
                .ReturnsAsync((Pet p) =>
                {
                    p.Id = 1;
                    return p;
                });

            var request = new PetRequest
            {
                NmPet = "Sauro",
                DsEspecie = "Gato",
                DsRaca = "SRD",
                NrIdade = 6,
                StCastrado = 1,
                TutorId = 1
            };

            // Act
            var resultado = await service.CriarAsync(request);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Sauro", resultado.NmPet);
            Assert.Equal(1, resultado.Id);

            petRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pet>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_SemNome_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var petRepositorioMock, out var tutorRepositorioMock);

            var request = new PetRequest { NmPet = "", DsEspecie = "Gato", DsRaca = "SRD", TutorId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            petRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_SemEspecie_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var petRepositorioMock, out var tutorRepositorioMock);

            var request = new PetRequest { NmPet = "Sauro", DsEspecie = "", DsRaca = "SRD", TutorId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            petRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_SemRaca_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var petRepositorioMock, out var tutorRepositorioMock);

            var request = new PetRequest { NmPet = "Sauro", DsEspecie = "Gato", DsRaca = "", TutorId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

            petRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_ComTutorInexistente_DeveLancarExcecao()
        {
            // Arrange
            var service = CriarService(out var petRepositorioMock, out var tutorRepositorioMock);

            tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tutor?)null);

            var request = new PetRequest { NmPet = "Sauro", DsEspecie = "Gato", DsRaca = "SRD", TutorId = 999 };

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CriarAsync(request));

            petRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComPetInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var petRepositorioMock, out var tutorRepositorioMock);

            petRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Pet?)null);

            var request = new PetRequest { NmPet = "Sauro", DsEspecie = "Gato", DsRaca = "SRD", TutorId = 1 };

            // Act
            var (sucesso, tutorNaoEncontrado) = await service.AtualizarAsync(999, request);

            // Assert
            Assert.False(sucesso);
            Assert.False(tutorNaoEncontrado);
        }

        [Fact]
        public async Task AtualizarAsync_ComTutorInexistente_DeveRetornarTutorNaoEncontrado()
        {
            // Arrange
            var service = CriarService(out var petRepositorioMock, out var tutorRepositorioMock);

            var petExistente = new Pet { Id = 1, NmPet = "Sauro" };
            petRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(petExistente);
            tutorRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Tutor?)null);

            var request = new PetRequest { NmPet = "Sauro", DsEspecie = "Gato", DsRaca = "SRD", TutorId = 999 };

            // Act
            var (sucesso, tutorNaoEncontrado) = await service.AtualizarAsync(1, request);

            // Assert
            Assert.False(sucesso);
            Assert.True(tutorNaoEncontrado);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_DeveRetornarFalso()
        {
            // Arrange
            var service = CriarService(out var petRepositorioMock, out var tutorRepositorioMock);

            petRepositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Pet?)null);

            // Act
            var resultado = await service.RemoverAsync(999);

            // Assert
            Assert.False(resultado);
        }
    }
}