using VetiWebApplication.Models;

namespace VetiWebApplication.Interfaces
{
    //Interface que define quais operações podem ser realizadas no repositório de tratamento.
    public interface ITratamentoRepository
    {
        
        //Obtem todos os tratamentos
        Task<IEnumerable<Tratamento>> ObterTodosAsync();

        //Obtem um tratamento específico por ID.
        Task<Tratamento?> ObterPorIdAsync(int id);

        //Obtem tratamento de um pet específico.
        Task<IEnumerable<Tratamento>> ObterPorPetAsync(int petId);
        
        //Adiciona um novo tratamento.
        Task<Tratamento> AdicionarAsync(Tratamento tratamento);

        //Atualiza as informações de um tratamento.
        Task AtualizarAsync(Tratamento tratamento);

        //Remove um tratamento
        Task RemoverAsync(Tratamento tratamento);
    }
}
