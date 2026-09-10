using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;

namespace VetiWebApplication.Services
{
    // Camada de regra de negócio para medicamentos relacionados a exames.
    public class ExameMedicamentoService
    {
        // Repositório de consultas para operações de CRUD.
        private readonly IExameMedicamentoRepository dbRepository;

        //Repositórios de exames para validação.
        private readonly IExameRepository dbExameRepository;


        //Repositórios de medicamentos para validação.
        private readonly IMedicamentoRepository dbMedicamentoRepository;

        // Logger para registrar eventos importantes.
        private readonly ILogger<ExameMedicamentoService> dbLogger;

        public ExameMedicamentoService(
            IExameMedicamentoRepository repository,
            IExameRepository exameRepository,
            IMedicamentoRepository medicamentoRepository,
            ILogger<ExameMedicamentoService> logger)
        {
            dbRepository = repository;
            dbExameRepository = exameRepository;
            dbMedicamentoRepository = medicamentoRepository;
            dbLogger = logger;
        }


        //Método para obter todos os medicamentos e exames relacionados.
        public async Task<IEnumerable<ExameMedicamento>> ObterTodosAsync()
        {
            return await dbRepository.ObterTodosAsync();
        }

        //Método para obter medicamentos relacionados a um exame específico.
        public async Task<(bool exameExiste, IEnumerable<ExameMedicamento> lista)> ObterPorExameAsync(int exameId)
        {
            var exame = await dbExameRepository.ObterPorIdAsync(exameId);
            if (exame == null) return (false, Enumerable.Empty<ExameMedicamento>());

            var lista = await dbRepository.ObterPorExameAsync(exameId);
            return (true, lista);
        }


        //Método para obter exames relacionados a um medicamento específico.
        public async Task<(bool medicamentoExiste, IEnumerable<ExameMedicamento> lista)> ObterPorMedicamentoAsync(int medicamentoId)
        {
            var medicamento = await dbMedicamentoRepository.ObterPorIdAsync(medicamentoId);
            if (medicamento == null) return (false, Enumerable.Empty<ExameMedicamento>());

            var lista = await dbRepository.ObterPorMedicamentoAsync(medicamentoId);
            return (true, lista);
        }

        //Método para vincular um medicamento a um exame.
        public async Task<ExameMedicamento> CriarAsync(ExameMedicamentoRequest request)
        {
            //Valida se o exame existe
            var exame = await dbExameRepository.ObterPorIdAsync(request.ExameId);
            if (exame == null)
            {
                dbLogger.LogWarning("Tentativa de vincular medicamento a exame inexistente: {ExameId}", request.ExameId);
                throw new KeyNotFoundException($"Exame com ID {request.ExameId} não encontrado.");
            }


            //Valida se o medicamento existe
            var medicamento = await dbMedicamentoRepository.ObterPorIdAsync(request.MedicamentoId);
            if (medicamento == null)
            {
                dbLogger.LogWarning("Tentativa de vincular exame a medicamento inexistente: {MedicamentoId}", request.MedicamentoId);
                throw new KeyNotFoundException($"Medicamento com ID {request.MedicamentoId} não encontrado.");
            }

            //Verifica se a relação já existe
            var jaExiste = await dbRepository.ExisteAsync(request.ExameId, request.MedicamentoId);
            if (jaExiste)
            {
                dbLogger.LogWarning("Tentativa de vincular medicamento já vinculado: exame {ExameId}, medicamento {MedicamentoId}", request.ExameId, request.MedicamentoId);
                throw new InvalidOperationException("Este medicamento já está vinculado a este exame.");
            }

            var exameMedicamento = new ExameMedicamento
            {
                ExameId = request.ExameId,
                MedicamentoId = request.MedicamentoId,
                QtMedicamento = request.QtMedicamento
            };

            var criado = await dbRepository.AdicionarAsync(exameMedicamento);

            dbLogger.LogInformation("Medicamento {MedicamentoId} vinculado ao exame {ExameId}", request.MedicamentoId, request.ExameId);

            return criado;
        }

        //Método que obtem se existe uma relação entre um exame e um medicamento específicos
        public async Task<bool> RemoverAsync(int exameId, int medicamentoId)
        {
            var exameMedicamento = await dbRepository.ObterPorChaveAsync(exameId, medicamentoId);
            if (exameMedicamento == null) return false;

            await dbRepository.RemoverAsync(exameMedicamento);
            return true;
        }
    }
}