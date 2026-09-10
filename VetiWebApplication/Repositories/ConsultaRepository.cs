using VetiWebApplication.Data;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories
{

    //Implementação real, que lê e grava os dados de Consulta no banco Oracle.
    public class ConsultaRepository : IConsultaRepository
    {

        //Contexto do banco de dados para acessar a tabela de Consultas.
        private readonly AppDbContext dbContext;

        public ConsultaRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        //Método para obter todas as consultas do banco de dados.
        public async Task<IEnumerable<Consulta>> ObterTodasAsync()
        {
            return await dbContext.Consultas
                .Include(consulta => consulta.Pet)
                .Include(consulta => consulta.Veterinario)
                .ToListAsync();
        }

        //Método para obter uma consulta pelo seu ID.
        public async Task<Consulta?> ObterPorIdAsync(int id)
        {
            return await dbContext.Consultas
                .Include(consulta => consulta.Pet).ThenInclude(pet => pet.Tutor)
                .Include(consulta => consulta.Veterinario)
                .FirstOrDefaultAsync(consulta => consulta.Id == id);
        }

        //Método para obter as consultas de um pet específico pelo seu ID.
        public async Task<IEnumerable<Consulta>> ObterPorPetAsync(int petId)
        {
            return await dbContext.Consultas
                .Include(consulta => consulta.Pet)
                .Include(consulta => consulta.Veterinario)
                .Where(consulta => consulta.PetId == petId)
                .ToListAsync();
        }

        //Método para adicionar uma nova consulta ao banco de dados.
        public async Task<Consulta> AdicionarAsync(Consulta consulta)
        {
            dbContext.Consultas.Add(consulta);
            await dbContext.SaveChangesAsync();
            return consulta;
        }


        //Método para atualizar informações de uma consulta existente.
        public async Task AtualizarAsync(Consulta consulta)
        {
            await dbContext.SaveChangesAsync();
        }


        //Método para remover uma consulta do banco de dados.
        public async Task RemoverAsync(Consulta consulta)
        {
            dbContext.Consultas.Remove(consulta);
            await dbContext.SaveChangesAsync();
        }
    }
}
