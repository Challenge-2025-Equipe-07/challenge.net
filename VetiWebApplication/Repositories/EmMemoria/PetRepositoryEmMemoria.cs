using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories.EmMemoria
{
    //Classe de repositório de Pets em memória para testes e desenvolvimento.
    public class PetRepositoryEmMemoria : IPetRepository
    {

        // Lista de pets armazenados em memória.
        private readonly List<Pet> dbPets = new();

        // Contador para gerar IDs únicos para os pets.
        private int dbProximoId = 1;


        //Método para obter todos os pets armazenados em memória.
        public Task<IEnumerable<Pet>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<Pet>>(dbPets);
        }

        // Método para obter um pet pelo seu ID.
        public Task<Pet?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(dbPets.FirstOrDefault(pet => pet.Id == id));
        }

        // Método para obter todos os pets de um tutor específico.
        public Task<IEnumerable<Pet>> ObterPorTutorAsync(int tutorId)
        {
            return Task.FromResult<IEnumerable<Pet>>(dbPets.Where(pet => pet.TutorId == tutorId).ToList());
        }


        //Método para obter todos os pets de uma espécie específica.
        public Task<IEnumerable<Pet>> ObterPorEspecieAsync(string especie)
        {
            return Task.FromResult<IEnumerable<Pet>>(
                dbPets.Where(pet => pet.DsEspecie.ToLower() == especie.ToLower()).ToList());
        }

        //Método para adicionar um novo pet.
        public Task<Pet> AdicionarAsync(Pet pet)
        {
            pet.Id = dbProximoId++;
            dbPets.Add(pet);
            return Task.FromResult(pet);
        }

        //Método para atualizar as informações de um pet existente.
        public Task AtualizarAsync(Pet pet)
        {
            return Task.CompletedTask;
        }

        //Método para remover um pet da lista de pets armazenados em memória.
        public Task RemoverAsync(Pet pet)
        {
            dbPets.Remove(pet);
            return Task.CompletedTask;
        }
    }
}