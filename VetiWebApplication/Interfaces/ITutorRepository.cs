using VetiWebApplication.Models;

namespace VetiWebApplication.Interfaces
{
    //Interface que define quais operações podem ser realizadas no repositório de Tutores.
    public interface ITutorRepository
    {
        //Obtem todos os tutores cadastrados.
        Task<IEnumerable<Tutor>> ObterTodosAsync();

        //Obtem um tutor específico pelo seu ID.
        Task<Tutor?> ObterPorIdAsync(int id);

        //Obtem um tutor específico pelo seu CPF.
        Task<Tutor?> ObterPorCpfAsync(string cpf);

        //Adiciona um novo tutor ao repositório.
        Task<Tutor> AdicionarAsync(Tutor tutor);

        //Atualiza as informações de um tutor existente.
        Task AtualizarAsync(Tutor tutor);

        //Remove um tutor do repositório.
        Task RemoverAsync(Tutor tutor);
    }
}
