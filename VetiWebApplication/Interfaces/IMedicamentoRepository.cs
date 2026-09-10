using VetiWebApplication.Models;

namespace VetiWebApplication.Interfaces
{

    //Interface que define quais operações podem ser realizadas no repositório de Medicamentos.
    public interface IMedicamentoRepository
    {

        //Obtem todos os medicamentos cadastrados.
        Task<IEnumerable<Medicamento>> ObterTodosAsync();

        //Obtem um medicamento específico pelo seu ID.
        Task<Medicamento?> ObterPorIdAsync(int id);

        //Adiciona um novo medicamento.
        Task<Medicamento> AdicionarAsync(Medicamento medicamento);

        //Atualiza as informações de um medicamento.
        Task AtualizarAsync(Medicamento medicamento);

        //Remove um medicamento.
        Task RemoverAsync(Medicamento medicamento);
    }
}
