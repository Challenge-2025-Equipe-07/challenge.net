using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;

namespace VetiWebApplication.Services
{
    // Camada de regra de negócio para Consulta.
    public class ConsultaService
    {
        // Repositório de consultas para operações de CRUD.
        private readonly IConsultaRepository dbRepository;

        // Repositório de pets para validar a existência do pet ao criar ou atualizar uma consulta.
        private readonly IPetRepository dbPetRepository;

        // Repositório de veterinários para validar a existência do veterinário ao criar ou atualizar uma consulta.
        private readonly IVeterinarioRepository dbVeterinarioRepository;

        // Logger para registrar eventos importantes.
        private readonly ILogger<ConsultaService> dbLogger;

        public ConsultaService(
            IConsultaRepository repository,
            IPetRepository petRepository,
            IVeterinarioRepository veterinarioRepository,
            ILogger<ConsultaService> logger)
        {
            dbRepository = repository;
            dbPetRepository = petRepository;
            dbVeterinarioRepository = veterinarioRepository;
            dbLogger = logger;
        }


        //Método para obter todas as consultas.
        public async Task<IEnumerable<Consulta>> ObterTodasAsync()
        {
            return await dbRepository.ObterTodasAsync();
        }

        //Método para obter uma consulta pelo seu ID.
        public async Task<Consulta?> ObterPorIdAsync(int id)
        {
            return await dbRepository.ObterPorIdAsync(id);
        }


        //Método para obter as consultas de um pet específico pelo seu ID.
        public async Task<IEnumerable<Consulta>> ObterPorPetAsync(int petId)
        {
            return await dbRepository.ObterPorPetAsync(petId);
        }


        //Método para adicionar uma nova consulta.
        public async Task<Consulta> CriarAsync(ConsultaRequest request)
        {
            //O tipo de evento é obrigatório
            if (string.IsNullOrEmpty(request.TpEvento))
            {
                dbLogger.LogWarning("Tentativa de criar consulta sem tipo de evento.");
                throw new ArgumentException("Tipo do evento é obrigatório.");
            }

            //Validando se o pet existe
            var pet = await dbPetRepository.ObterPorIdAsync(request.PetId);
            if (pet == null)
            {
                dbLogger.LogWarning("Tentativa de criar consulta com PetId inexistente: {PetId}", request.PetId);
                throw new KeyNotFoundException($"Pet com ID {request.PetId} não encontrado.");
            }

            //Valida se o veterinário existe
            var vet = await dbVeterinarioRepository.ObterPorIdAsync(request.VeterinarioId);
            if (vet == null)
            {
                dbLogger.LogWarning("Tentativa de criar consulta com VeterinarioId inexistente: {VeterinarioId}", request.VeterinarioId);
                throw new KeyNotFoundException($"Veterinário com ID {request.VeterinarioId} não encontrado.");
            }

            var consulta = new Consulta
            {
                DtConsulta = request.DtConsulta,
                TpEvento = request.TpEvento,
                Notificar = request.Notificar,
                PetId = request.PetId,
                VeterinarioId = request.VeterinarioId
            };

            var consultaCriada = await dbRepository.AdicionarAsync(consulta);

            dbLogger.LogInformation("Nova consulta criada para o pet {PetId}", consultaCriada.PetId);

            return consultaCriada;
        }

        //Método para atualizar informações de uma consulta existente.
        public async Task<(bool sucesso, bool veterinarioNaoEncontrado)> AtualizarAsync(int id, ConsultaRequest request)
        {
            var consulta = await dbRepository.ObterPorIdAsync(id);
            if (consulta == null) return (false, false);

            var vet = await dbVeterinarioRepository.ObterPorIdAsync(request.VeterinarioId);
            if (vet == null) return (false, true);

            consulta.DtConsulta = request.DtConsulta;
            consulta.TpEvento = request.TpEvento;
            consulta.Notificar = request.Notificar;
            consulta.PetId = request.PetId;
            consulta.VeterinarioId = request.VeterinarioId;

            await dbRepository.AtualizarAsync(consulta);
            return (true, false);
        }

        //Método para remover uma consulta.
        public async Task<bool> RemoverAsync(int id)
        {
            var consulta = await dbRepository.ObterPorIdAsync(id);
            if (consulta == null) return false;

            await dbRepository.RemoverAsync(consulta);
            return true;
        }
    }
}
