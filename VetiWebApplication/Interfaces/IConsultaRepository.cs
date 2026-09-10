using VetiWebApplication.Models;

namespace VetiWebApplication.Interfaces
{
    //Interface que define quais operações podem ser realizadas no repositório de Consultas.
    public interface IConsultaRepository
    {

        //Obtem todas as consultas cadastradas.
        Task<IEnumerable<Consulta>> ObterTodasAsync();

        //Obtem uma consulta específica por ID.
        Task<Consulta?> ObterPorIdAsync(int id);

        //Obtem todas as consultas de um pet específico por ID.
        Task<IEnumerable<Consulta>> ObterPorPetAsync(int petId);

        //Adiciona uma nova consulta ao repositório.
        Task<Consulta> AdicionarAsync(Consulta consulta);

        //Atualiza as informações de uma consulta existente no repositório.
        Task AtualizarAsync(Consulta consulta);

        //Remove uma consulta do repositório.
        Task RemoverAsync(Consulta consulta);
    }
}
