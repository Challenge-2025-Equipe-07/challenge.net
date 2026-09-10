using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;

namespace VetiWebApplication.Services
{

    // Camada de regra de negócio para Medicamentos.
    public class MedicamentoService
    {

        // Repositório de medicamentos para operações de CRUD.
        private readonly IMedicamentoRepository dbRepository;

        // Logger para registrar eventos importantes.
        private readonly ILogger<MedicamentoService> dbLogger;

        public MedicamentoService(IMedicamentoRepository repository, ILogger<MedicamentoService> logger)
        {
            dbRepository = repository;
            dbLogger = logger;
        }

        //Método que obtem todos os medicamentos.
        public async Task<IEnumerable<Medicamento>> ObterTodosAsync()
        {
            return await dbRepository.ObterTodosAsync();
        }


        //Método que obtem um medicameto específico pelo seu ID.
        public async Task<Medicamento?> ObterPorIdAsync(int id)
        {
            return await dbRepository.ObterPorIdAsync(id);
        }

        //Método que adiciona um novo medicamento.
        public async Task<Medicamento> CriarAsync(MedicamentoRequest request)
        {
            //O nome do medicamento é obrigatório
            if (string.IsNullOrEmpty(request.NmMedicamento))
            {
                dbLogger.LogWarning("Tentativa de criar medicamento sem nome.");
                throw new ArgumentException("Nome do medicamento é obrigatório.");
            }

            //A dosagem do medicamento é obrigatória
            if (string.IsNullOrEmpty(request.DsDosagem))
            {
                dbLogger.LogWarning("Tentativa de criar medicamento sem dosagem.");
                throw new ArgumentException("Dosagem é obrigatória.");
            }

            //A frequência do uso do medicamento é obrigatória.
            if (string.IsNullOrEmpty(request.DsFrequencia))
            {
                dbLogger.LogWarning("Tentativa de criar medicamento sem frequência.");
                throw new ArgumentException("Frequência é obrigatória.");
            }

            var medicamento = new Medicamento
            {
                NmMedicamento = request.NmMedicamento,
                DsDosagem = request.DsDosagem,
                DsFrequencia = request.DsFrequencia
            };

            var medicamentoCriado = await dbRepository.AdicionarAsync(medicamento);

            dbLogger.LogInformation("Novo medicamento criado: {NomeMedicamento}", medicamentoCriado.NmMedicamento);

            return medicamentoCriado;
        }



        //Método que atualiza as informãções de um medicamento existente.
        public async Task<bool> AtualizarAsync(int id, MedicamentoRequest request)
        {
            var medicamento = await dbRepository.ObterPorIdAsync(id);
            if (medicamento == null) return false;

            medicamento.NmMedicamento = request.NmMedicamento;
            medicamento.DsDosagem = request.DsDosagem;
            medicamento.DsFrequencia = request.DsFrequencia;

            await dbRepository.AtualizarAsync(medicamento);
            return true;
        }

        //Método que remove um medicamento do banco de dados.
        public async Task<bool> RemoverAsync(int id)
        {
            var medicamento = await dbRepository.ObterPorIdAsync(id);
            if (medicamento == null) return false;

            await dbRepository.RemoverAsync(medicamento);
            return true;
        }
    }
}