using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories.EmMemoria
{
    //Classe de repositório de Veterinários em memória para testes e desenvolvimento.
    public class VeterinarioRepositoryEmMemoria : IVeterinarioRepository
    {

        // Lista de veterinários armazenados em memória.
        private readonly List<Veterinario> dbVeterinarios = new();

        // Contador para gerar IDs únicos para os veterinários.
        private int dbProximoId = 1;


        //Método para obter todos os veterinários armazenados em memória.
        public Task<IEnumerable<Veterinario>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<Veterinario>>(dbVeterinarios);
        }


        //Método para obter um veterinário específico pelo seu ID.
        public Task<Veterinario?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(dbVeterinarios.FirstOrDefault(veterinario => veterinario.Id == id));
        }


        //Método para adicionar um novo veterinário à lista em memória.
        public Task<Veterinario> AdicionarAsync(Veterinario veterinario)
        {
            veterinario.Id = dbProximoId++;
            dbVeterinarios.Add(veterinario);
            return Task.FromResult(veterinario);
        }

        //Método para atualizar um veterinário existente na lista em memória.
        public Task AtualizarAsync(Veterinario veterinario)
        {
            return Task.CompletedTask;
        }


        //Método para remover um veterinário da lista em memória.
        public Task RemoverAsync(Veterinario veterinario)
        {
            dbVeterinarios.Remove(veterinario);
            return Task.CompletedTask;
        }
    }
}