using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;

namespace VetiWebApplication.Services
{
    // Camada de regra de negócio para Tratamentos.
    public class TratamentoService
    {

        // Repositório de medicamentos para operações de CRUD.
        private readonly ITratamentoRepository dbRepository;

        // Repositório de relação entre tratamento e medicamento.
        private readonly ITratamentoMedicamentoRepository dbTratamentoMedicamentoRepository;

        // Repositório de pets para validação
        private readonly IPetRepository dbPetRepository;

        //Repositório de medicamento para validação.
        private readonly IMedicamentoRepository dbMedicamentoRepository;

        // Logger para registrar eventos importantes.
        private readonly ILogger<TratamentoService> dbLogger;

        public TratamentoService(
            ITratamentoRepository repository,
            ITratamentoMedicamentoRepository tratamentoMedicamentoRepository,
            IPetRepository petRepository,
            IMedicamentoRepository medicamentoRepository,
            ILogger<TratamentoService> logger)
        {
            dbRepository = repository;
            dbTratamentoMedicamentoRepository = tratamentoMedicamentoRepository;
            dbPetRepository = petRepository;
            dbMedicamentoRepository = medicamentoRepository;
            dbLogger = logger;
        }


        //Método para obter todos os tratamentos do banco de dados.
        public async Task<IEnumerable<Tratamento>> ObterTodosAsync()
        {
            return await dbRepository.ObterTodosAsync();
        }


        //Método para obter um tratamento específico.
        public async Task<Tratamento?> ObterPorIdAsync(int id)
        {
            return await dbRepository.ObterPorIdAsync(id);
        }


        //Método para obter o tratamento de um pet específico.
        public async Task<(bool petExiste, IEnumerable<Tratamento> lista)> ObterPorPetAsync(int petId)
        {
            var pet = await dbPetRepository.ObterPorIdAsync(petId);
            if (pet == null) return (false, Enumerable.Empty<Tratamento>());

            var lista = await dbRepository.ObterPorPetAsync(petId);
            return (true, lista);
        }


        //Método para adicionar um novo tratamento ao banco de dados.
        public async Task<Tratamento> CriarAsync(TratamentoRequest request)
        {
            //O diagnóstico é obrigatório
            if (string.IsNullOrEmpty(request.DsDiagnostico))
            {
                dbLogger.LogWarning("Tentativa de criar tratamento sem diagnóstico.");
                throw new ArgumentException("Diagnóstico é obrigatório.");
            }

            //Valida se o pet existe
            var pet = await dbPetRepository.ObterPorIdAsync(request.PetId);
            if (pet == null)
            {
                dbLogger.LogWarning("Tentativa de criar tratamento com PetId inexistente: {PetId}", request.PetId);
                throw new KeyNotFoundException($"Pet com ID {request.PetId} não encontrado.");
            }

            var tratamento = new Tratamento
            {
                DsDiagnostico = request.DsDiagnostico,
                DtInicio = request.DtInicio,
                DtRetornoPrevisto = request.DtRetornoPrevisto,
                DsObservacao = request.DsObservacao,
                PetId = request.PetId
            };

            var tratamentoCriado = await dbRepository.AdicionarAsync(tratamento);

            dbLogger.LogInformation("Novo tratamento criado: {Diagnostico}", tratamentoCriado.DsDiagnostico);

            return tratamentoCriado;
        }

        //Método para vincular um medicamento a um tratamento
        public async Task<TratamentoMedicamento> AdicionarMedicamentoAsync(int tratamentoId, TratamentoMedicamentoRequest request)
        {
            //Valida se o tratamento existe
            var tratamento = await dbRepository.ObterPorIdAsync(tratamentoId);
            if (tratamento == null)
            {
                dbLogger.LogWarning("Tentativa de adicionar medicamento a tratamento inexistente: {TratamentoId}", tratamentoId);
                throw new KeyNotFoundException("Tratamento não encontrado.");
            }

            //Valida se o medicamento existe
            var medicamento = await dbMedicamentoRepository.ObterPorIdAsync(request.MedicamentoId);
            if (medicamento == null)
            {
                dbLogger.LogWarning("Tentativa de vincular medicamento inexistente: {MedicamentoId}", request.MedicamentoId);
                throw new KeyNotFoundException($"Medicamento com ID {request.MedicamentoId} não encontrado.");
            }

            var jaExiste = await dbTratamentoMedicamentoRepository.ExisteAsync(tratamentoId, request.MedicamentoId);
            if (jaExiste)
            {
                dbLogger.LogWarning("Tentativa de vincular medicamento já vinculado: tratamento {TratamentoId}, medicamento {MedicamentoId}", tratamentoId, request.MedicamentoId);
                throw new InvalidOperationException("Este medicamento já está vinculado a este tratamento.");
            }

            var tratamentoMedicamento = new TratamentoMedicamento
            {
                TratamentoId = tratamentoId,
                MedicamentoId = request.MedicamentoId,
                QtMedicamento = request.QtMedicamento,
                DsInstrucao = request.DsInstrucao
            };

            var criado = await dbTratamentoMedicamentoRepository.AdicionarAsync(tratamentoMedicamento);

            dbLogger.LogInformation("Medicamento {MedicamentoId} adicionado ao tratamento {TratamentoId}", request.MedicamentoId, tratamentoId);

            return criado;
        }

        //Método para atualizar as informações de um tratamento existente no banco de dados
        public async Task<bool> AtualizarAsync(int id, TratamentoRequest request)
        {
            var tratamento = await dbRepository.ObterPorIdAsync(id);
            if (tratamento == null) return false;

            tratamento.DsDiagnostico = request.DsDiagnostico;
            tratamento.DtInicio = request.DtInicio;
            tratamento.DtRetornoPrevisto = request.DtRetornoPrevisto;
            tratamento.DsObservacao = request.DsObservacao;
            tratamento.PetId = request.PetId;

            await dbRepository.AtualizarAsync(tratamento);
            return true;
        }

        //Método para remover um tratamento do banco de dados.
        public async Task<bool> RemoverAsync(int id)
        {
            var tratamento = await dbRepository.ObterPorIdAsync(id);
            if (tratamento == null) return false;

            await dbRepository.RemoverAsync(tratamento);
            return true;
        }
    }
}