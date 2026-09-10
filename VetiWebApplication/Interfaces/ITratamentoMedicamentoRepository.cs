using VetiWebApplication.Models;

namespace VetiWebApplication.Interfaces
{
    //Interface que define quais operações podem ser realizadas
    //no repositório de medicamento vinculado a tratamento.
    public interface ITratamentoMedicamentoRepository
    {
        //Verifica se a relação entre medicamento e tratamento existe.
        Task<bool> ExisteAsync(int tratamentoId, int medicamentoId);

        //Adiciona uma relação entre medicamento e tratamento.
        Task<TratamentoMedicamento> AdicionarAsync(TratamentoMedicamento tratamentoMedicamento);
    }
}