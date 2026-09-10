using VetiWebApplication.Models;



namespace VetiWebApplication.Interfaces
{
    //Interface que define quais operações podem ser realizadas no repositório de Veterinários.
    public interface IVeterinarioRepository
    {
        //Obtem todos os veterinários.
        Task<IEnumerable<Veterinario>> ObterTodosAsync();

        //Obtem um veterinário específico por ID.
        Task<Veterinario?> ObterPorIdAsync(int id);

        //Adiciona um novo veterinário ao repositório.
        Task<Veterinario> AdicionarAsync(Veterinario veterinario);

        //Atualiza as informações de um veterinário existente.
        Task AtualizarAsync(Veterinario veterinario);

        //Remove um veterinário do repositório.
        Task RemoverAsync(Veterinario veterinario);
    }
}
