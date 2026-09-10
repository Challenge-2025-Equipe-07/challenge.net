using VetiWebApplication.Models;

namespace VetiWebApplication.Interfaces
{
    //Interface que define quais operações podem ser realizadas no repositório de Exames.
    public interface IExameRepository
    {
        //Obtem todos os exames cadastrados.
        Task<IEnumerable<Exame>> ObterTodosAsync();

        //Obtem um exame por seu ID.
        Task<Exame?> ObterPorIdAsync(int id);

        //Obtem um exame pelo ID de uma consulta específica
        Task<IEnumerable<Exame>> ObterPorConsultaAsync(int consultaId);

        //Obtem os exames de um pet específico pelo seu ID.
        Task<IEnumerable<Exame>> ObterPorPetAsync(int petId);

        //Adiciona um novo exame.
        Task<Exame> AdicionarAsync(Exame exame);

        //Atualiza as informações de um exame existente.
        Task AtualizarAsync(Exame exame);

        //Remove um exame existente.
        Task RemoverAsync(Exame exame);
    }
}
