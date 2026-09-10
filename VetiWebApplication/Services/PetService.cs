using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;

namespace VetiWebApplication.Services
{
    // Camada de regra de negócio para Pet.
    public class PetService
    {
        // Repositório de pets para operações de CRUD.
        private readonly IPetRepository dbRepository;
        // Repositório de tutores para validar a existência do tutor ao criar ou atualizar um pet.
        private readonly ITutorRepository dbTutorRepository;
        // Logger para registrar eventos importantes.
        private readonly ILogger<PetService> dbLogger;

        public PetService(IPetRepository repository, ITutorRepository tutorRepository, ILogger<PetService> logger)
        {
            dbRepository = repository;
            dbTutorRepository = tutorRepository;
            dbLogger = logger;
        }

        // Método para obter todos os pets cadastrados.
        public async Task<IEnumerable<Pet>> ObterTodosAsync()
        {
            return await dbRepository.ObterTodosAsync();
        }

        // Método para obter um pet específico pelo seu ID.
        public async Task<Pet?> ObterPorIdAsync(int id)
        {
            return await dbRepository.ObterPorIdAsync(id);
        }

        // Método para obter todos os pets de um tutor específico pelo ID do tutor.
        public async Task<IEnumerable<Pet>> ObterPorTutorAsync(int tutorId)
        {
            return await dbRepository.ObterPorTutorAsync(tutorId);
        }


        // Método para obter todos os pets de uma espécie específica.
        public async Task<IEnumerable<Pet>> ObterPorEspecieAsync(string especie)
        {
            return await dbRepository.ObterPorEspecieAsync(especie);
        }

        // Método para criar um novo pet, validando os dados fornecidos e verificando a existência do tutor.
        public async Task<Pet> CriarAsync(PetRequest request)
        {
            // Nome do pet é obrigatório
            if (string.IsNullOrEmpty(request.NmPet))
            {
                dbLogger.LogWarning("Tentativa de criar pet sem nome.");
                throw new ArgumentException("Nome do pet é obrigatório.");
            }

            // Espécie do pet é obrigatória

            if (string.IsNullOrEmpty(request.DsEspecie))
            {
                dbLogger.LogWarning("Tentativa de criar pet sem espécie.");
                throw new ArgumentException("Espécie é obrigatória.");
            }

            // Raça do pet é obrigatória
            if (string.IsNullOrEmpty(request.DsRaca))
            {
                dbLogger.LogWarning("Tentativa de criar pet sem raça.");
                throw new ArgumentException("Raça é obrigatória.");
            }

            var tutor = await dbTutorRepository.ObterPorIdAsync(request.TutorId);
            if (tutor == null)
            { 
                dbLogger.LogWarning("Tentativa de criar pet com TutorId inexistente: {TutorId}", request.TutorId);
                throw new KeyNotFoundException($"Tutor com ID {request.TutorId} não encontrado.");
            }

            var pet = new Pet
            {
                NmPet = request.NmPet,
                DsEspecie = request.DsEspecie,
                DsRaca = request.DsRaca,
                NrIdade = request.NrIdade,
                StCastrado = request.StCastrado,
                TutorId = request.TutorId
            };

            var petCriado = await dbRepository.AdicionarAsync(pet);

            dbLogger.LogInformation("Novo pet criado: {NomePet}", petCriado.NmPet);

            return petCriado;
        }

        // Método para atualizar os dados de um pet existente, validando a existência do pet e do tutor.
        public async Task<(bool sucesso, bool tutorNaoEncontrado)> AtualizarAsync(int id, PetRequest request)
        {
            var pet = await dbRepository.ObterPorIdAsync(id);
            if (pet == null) return (false, false);

            var tutor = await dbTutorRepository.ObterPorIdAsync(request.TutorId);
            if (tutor == null) return (false, true);

            pet.NmPet = request.NmPet;
            pet.DsEspecie = request.DsEspecie;
            pet.DsRaca = request.DsRaca;
            pet.NrIdade = request.NrIdade;
            pet.StCastrado = request.StCastrado;
            pet.TutorId = request.TutorId;

            await dbRepository.AtualizarAsync(pet);
            return (true, false);
        }

        // Método para remover um pet existente pelo seu ID, validando a existência do pet.
        public async Task<bool> RemoverAsync(int id)
        {
            var pet = await dbRepository.ObterPorIdAsync(id);
            if (pet == null) return false;

            await dbRepository.RemoverAsync(pet);
            return true;
        }
    }
}