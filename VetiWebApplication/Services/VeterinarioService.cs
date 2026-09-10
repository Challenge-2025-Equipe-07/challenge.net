using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;

namespace VetiWebApplication.Services
{
    // Camada de regra de negócio para Veterinário.
    public class VeterinarioService
    {
        // Repositório de veterinários para operações de CRUD.
        private readonly IVeterinarioRepository dbRepository;

        // Logger para registrar eventos importantes.
        private readonly ILogger<VeterinarioService> dbLogger;


        public VeterinarioService(IVeterinarioRepository repository, ILogger<VeterinarioService> logger)
        {
            dbRepository = repository;
            dbLogger = logger;
        }


        //Método para obter todos os veterinários.
        public async Task<IEnumerable<Veterinario>> ObterTodosAsync()
        {
            return await dbRepository.ObterTodosAsync();
        }

        //Método para obter um veterinário por ID.

        public async Task<Veterinario?> ObterPorIdAsync(int id)
        {
            return await dbRepository.ObterPorIdAsync(id);
        }


        //Método para criar um novo veterinário.
        public async Task<Veterinario> CriarAsync(VeterinarioRequest request)
        {

            //Email é obrigatório
            if (string.IsNullOrEmpty(request.DsEmail))
            {
                dbLogger.LogWarning("Tentativa de criar veterinário sem email.");
                throw new ArgumentException("Email é obrigatório.");
            }

            //Senha é obrigatória
            if (string.IsNullOrEmpty(request.DsPassword))
            {
                dbLogger.LogWarning("Tentativa de criar veterinário sem senha.");
                throw new ArgumentException("Senha é obrigatória.");
            }

            var veterinario = new Veterinario
            {
                DsEmail = request.DsEmail,
                DsPassword = request.DsPassword
            };

            var veterinarioCriado = await dbRepository.AdicionarAsync(veterinario);

            dbLogger.LogInformation("Novo veterinário criado: {Email}", veterinarioCriado.DsEmail);

            return veterinarioCriado;
        }

        //Método para atualizar um veterinário existente.
        public async Task<bool> AtualizarAsync(int id, VeterinarioRequest request)
        {
            var veterinario = await dbRepository.ObterPorIdAsync(id);
            if (veterinario == null) return false;

            veterinario.DsEmail = request.DsEmail;
            veterinario.DsPassword = request.DsPassword;

            await dbRepository.AtualizarAsync(veterinario);
            return true;
        }

        //Método para remover um veterinário existente.
        public async Task<bool> RemoverAsync(int id)
        {
            var veterinario = await dbRepository.ObterPorIdAsync(id);
            if (veterinario == null) return false;

            await dbRepository.RemoverAsync(veterinario);
            return true;
        }
    }
}