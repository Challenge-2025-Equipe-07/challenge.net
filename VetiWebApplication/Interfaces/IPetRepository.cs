using VetiWebApplication.Models;

namespace VetiWebApplication.Interfaces
{
    //Interface que define quais operações podem ser realizadas no repositório de Tutores.
    public interface IPetRepository
    {

        //Obtem todos os pets cadastrados.
        Task<IEnumerable<Pet>> ObterTodosAsync();

        //Obtem um pet específico por ID.
        Task<Pet?> ObterPorIdAsync(int id);

        //Obtem um pet específico por ID do tutor.
        Task<IEnumerable<Pet>> ObterPorTutorAsync(int tutorId);

        //Obtem todos os pets de uma espécie específica.
        Task<IEnumerable<Pet>> ObterPorEspecieAsync(string especie);

        //Adiciona um novo Pet ao repositório.
        Task<Pet> AdicionarAsync(Pet pet);

        //Atualiza as informações de um pet existente no repositório.
        Task AtualizarAsync(Pet pet);

        //Remove um Pet do repositório.
        Task RemoverAsync(Pet pet);
    }
}
