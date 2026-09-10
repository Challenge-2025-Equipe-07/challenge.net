using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories.EmMemoria
{
    //Classe de repositório de Medicamenros de Exames em memória para testes e desenvolvimento.
    public class ExameMedicamentoRepositoryEmMemoria : IExameMedicamentoRepository
    {


        // Lista de medicamentos de exames armazenados em memória.
        private readonly List<ExameMedicamento> dbExameMedicamentos = new();

        //Método para obter todos os medicamentos e exames relacionados.
        public Task<IEnumerable<ExameMedicamento>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<ExameMedicamento>>(dbExameMedicamentos);
        }


        //Método para obter medicamentos relacionados a um exame específico.
        public Task<IEnumerable<ExameMedicamento>> ObterPorExameAsync(int exameId)
        {
            return Task.FromResult<IEnumerable<ExameMedicamento>>(
                dbExameMedicamentos.Where(em => em.ExameId == exameId).ToList());
        }

        //Método para obter exames relacionados a um medicamento específico.
        public Task<IEnumerable<ExameMedicamento>> ObterPorMedicamentoAsync(int medicamentoId)
        {
            return Task.FromResult<IEnumerable<ExameMedicamento>>(
                dbExameMedicamentos.Where(em => em.MedicamentoId == medicamentoId).ToList());
        }

        //Método para obter uma relação específica pelo ExameId + MedicamentoId.
        public Task<ExameMedicamento?> ObterPorChaveAsync(int exameId, int medicamentoId)
        {
            return Task.FromResult(dbExameMedicamentos
                .FirstOrDefault(em => em.ExameId == exameId && em.MedicamentoId == medicamentoId));
        }

        //Método que obtem se existe uma relação entre um exame e um medicamento específicos.
        public Task<bool> ExisteAsync(int exameId, int medicamentoId)
        {
            return Task.FromResult(dbExameMedicamentos
                .Any(em => em.ExameId == exameId && em.MedicamentoId == medicamentoId));
        }

        //Método para vincular um medicamento a um exame.
        public Task<ExameMedicamento> AdicionarAsync(ExameMedicamento exameMedicamento)
        {
            dbExameMedicamentos.Add(exameMedicamento);
            return Task.FromResult(exameMedicamento);
        }

        //Método para remover uma relação existente.
        public Task RemoverAsync(ExameMedicamento exameMedicamento)
        {
            dbExameMedicamentos.Remove(exameMedicamento);
            return Task.CompletedTask;
        }
    }
}