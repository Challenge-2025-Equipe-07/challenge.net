using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;

namespace VetiWebApplication.Services
{
    // Camada de regra de negócio para Tutor.
    public class TutorService
    {
        // Repositório de tutores para operações de CRUD.
        private readonly ITutorRepository dbRepository;

        // Logger para registrar eventos importantes.
        private readonly ILogger<TutorService> dbLogger;


        public TutorService(ITutorRepository repository, ILogger<TutorService> logger)
        {
            dbRepository = repository;
            dbLogger = logger;
        }


        // Método para obter todos os tutores cadastrados.
        public async Task<IEnumerable<Tutor>> ObterTodosAsync()
        {
            return await dbRepository.ObterTodosAsync();
        }


        // Método para obter um tutor específico pelo seu ID.
        public async Task<Tutor?> ObterPorIdAsync(int id)
        {
            return await dbRepository.ObterPorIdAsync(id);
        }


        // Método para obter um tutor específico pelo seu CPF.
        public async Task<Tutor?> ObterPorCpfAsync(string cpf)
        {
            return await dbRepository.ObterPorCpfAsync(cpf);
        }

        // Método para criar um novo tutor..
        public async Task<Tutor> CriarAsync(TutorRequest request)
        {
            // Nome do tutor é obrigatório.
            if (string.IsNullOrEmpty(request.NmTutor))
            {
                dbLogger.LogWarning("Tentativa de criar tutor sem nome.");
                throw new ArgumentException("Nome é obrigatório.");
            }

            // CPF do tutor é obrigatório.
            if (string.IsNullOrEmpty(request.DsCpf))
            {
                dbLogger.LogWarning("Tentativa de criar tutor sem CPF.");
                throw new ArgumentException("CPF é obrigatório.");
            }

            // Email do tutor é obrigatório.
            if (string.IsNullOrEmpty(request.DsTelefone))
            {
                dbLogger.LogWarning("Tentativa de criar tutor sem telefone.");
                throw new ArgumentException("Telefone é obrigatório.");
            }

            var tutor = new Tutor
            {
                NmTutor = request.NmTutor,
                DsCpf = request.DsCpf,
                DsEmail = request.DsEmail,
                DsTelefone = request.DsTelefone
            };

            var tutorCriado = await dbRepository.AdicionarAsync(tutor);

            dbLogger.LogInformation("Novo tutor criado: {NomeTutor}", tutorCriado.NmTutor);

            return tutorCriado;
        }

        // Método para atualizar os dados de um tutor existente.
        public async Task<bool> AtualizarAsync(int id, TutorRequest request)
        {
            var tutor = await dbRepository.ObterPorIdAsync(id);
            if (tutor == null) return false;

            tutor.NmTutor = request.NmTutor;
            tutor.DsCpf = request.DsCpf;
            tutor.DsEmail = request.DsEmail;
            tutor.DsTelefone = request.DsTelefone;

            await dbRepository.AtualizarAsync(tutor);
            return true;
        }

        // Método para remover um tutor existente.
        public async Task<bool> RemoverAsync(int id)
        {
            var tutor = await dbRepository.ObterPorIdAsync(id);
            if (tutor == null) return false;

            await dbRepository.RemoverAsync(tutor);
            return true;
        }
    }
}
