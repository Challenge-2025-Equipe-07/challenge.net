using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories
{
    //  Implementação real, que lê e grava os dados de Pet no banco Oracle.
    public class PetRepository : IPetRepository
    {
        // Contexto do banco de dados para acessar a tabela de Pets.
        private readonly AppDbContext dbContext;


        public PetRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;         
        }


        //Método para obter todos os pets do banco de dados.
        public async Task<IEnumerable<Pet>> ObterTodosAsync()
        {
            return await dbContext.Pets.ToListAsync();
        }


        //Método para obter um pet pelo seu ID.
        public async Task<Pet?> ObterPorIdAsync(int id)
        {
            return await dbContext.Pets.Include(pet => pet.Tutor).FirstOrDefaultAsync(pet => pet.Id == id);
        }


        //Método para obter todos os pets de um tutor específico.
        public async Task<IEnumerable<Pet>> ObterPorTutorAsync(int tutorId)
        {
            return await dbContext.Pets.Include(pet => pet.Tutor).Where(pet => pet.TutorId == tutorId).ToListAsync();
        }


        //Método para obter todos os pets de uma espécie específica.
        public async Task<IEnumerable<Pet>> ObterPorEspecieAsync(string especie)
        {
            return await dbContext.Pets.Include(pet => pet.Tutor)
                .Where(pet => pet.DsEspecie.ToLower() == especie.ToLower())
                .ToListAsync();
        }

        //Método para adicionar um novo pet ao banco de dados.
        public async Task<Pet> AdicionarAsync(Pet pet)
        {
            dbContext.Pets.Add(pet);
            await dbContext.SaveChangesAsync();
            return pet;
        }


        //Método para atualizar um pet existente no banco de dados.
        public async Task AtualizarAsync(Pet pet)
        {
            await dbContext.SaveChangesAsync();
        }


        //Método para remover um pet do banco de dados.
        public async Task RemoverAsync(Pet pet)
        {
            dbContext.Pets.Remove(pet);
            await dbContext.SaveChangesAsync();
        }

    }
}
