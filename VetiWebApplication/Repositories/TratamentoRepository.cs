using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories
{
    //Implementação real, que lê e grava os dados de Tratamento no banco Oracle.
    public class TratamentoRepository : ITratamentoRepository
    {
        //Contexto do banco de dados para acessar a tabela de Tratamento.
        private readonly AppDbContext dbContext;

        public TratamentoRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }


        //Método para obter todos os tratamentos do banco de dados
        public async Task<IEnumerable<Tratamento>> ObterTodosAsync()
        {
            return await dbContext.Tratamentos
                .Include(t => t.Pet)
                .Include(t => t.TratamentoMedicamentos).ThenInclude(tratamento => tratamento.Medicamento)
                .ToListAsync();
        }


        //Método para obter um tratamento específico.
        public async Task<Tratamento?> ObterPorIdAsync(int id)
        {
            return await dbContext.Tratamentos
                .Include(t => t.Pet)
                .Include(t => t.TratamentoMedicamentos).ThenInclude(tratamento => tratamento.Medicamento)
                .FirstOrDefaultAsync(t => t.Id == id);
        }


        //Método para obter o tratamento de um pet específico.
        public async Task<IEnumerable<Tratamento>> ObterPorPetAsync(int petId)
        {
            return await dbContext.Tratamentos
                .Include(t => t.TratamentoMedicamentos).ThenInclude(tratamento => tratamento.Medicamento)
                .Where(t => t.PetId == petId)
                .ToListAsync();
        }


        //Método para adicionar um novo tratamento ao banco de dados
        public async Task<Tratamento> AdicionarAsync(Tratamento tratamento)
        {
            dbContext.Tratamentos.Add(tratamento);
            await dbContext.SaveChangesAsync();
            return tratamento;
        }


        //Método para atualizar as informações de um tratamento existente no banco de dados
        public async Task AtualizarAsync(Tratamento tratamento)
        {
            await dbContext.SaveChangesAsync();
        }


        //Método para remover um tratamento do banco de dados.
        public async Task RemoverAsync(Tratamento tratamento)
        {
            dbContext.Tratamentos.Remove(tratamento);
            await dbContext.SaveChangesAsync();
        }
    }
}