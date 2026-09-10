using System.Runtime.Intrinsics.X86;
using VetiWebApplication.Models;

namespace VetiWebApplication.Interfaces
{
    //Interface que define quais operações podem ser realizadas no repositório de Medicamentos relacionados a um exame.
    public interface IExameMedicamentoRepository
    {

        //Obtem todos os medicamentos relacionados exames
        Task<IEnumerable<ExameMedicamento>> ObterTodosAsync();

        //Obtem os medicamentos relacionados a um exame específico por seu ID.
        Task<IEnumerable<ExameMedicamento>> ObterPorExameAsync(int exameId);

        //Obtem exames relacionados a um medicamento específico por ID.
        Task<IEnumerable<ExameMedicamento>> ObterPorMedicamentoAsync(int medicamentoId);

        //Busca uma relação específica pela combinação de ExameId + MedicamentoId
        Task<ExameMedicamento?> ObterPorChaveAsync(int exameId, int medicamentoId);

        //Verifica se já existe uma relação entre esse exame e esse medicamento
        Task<bool> ExisteAsync(int exameId, int medicamentoId);

        //Vincula um medicamento com um exame
        Task<ExameMedicamento> AdicionarAsync(ExameMedicamento exameMedicamento);

        //Remove uma relação existentes.
        Task RemoverAsync(ExameMedicamento exameMedicamento);
    }
}
