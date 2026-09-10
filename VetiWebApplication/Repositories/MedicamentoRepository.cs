using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories
{
    // Implementação real, que lê e grava os dados de Medicamento no banco Oracle.
    public class MedicamentoRepository : IMedicamentoRepository
    {
        // Contexto do banco de dados para acessar a tabela de Medicamentos.
        private readonly AppDbContext dbContext;

        public MedicamentoRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        //Método que obtem todos os medicamentos do banco de dados.
        public async Task<IEnumerable<Medicamento>> ObterTodosAsync()
        {
            return await dbContext.Medicamentos.ToListAsync();
        }

        //Método que obtem um medicameto específico pelo seu ID.
        public async Task<Medicamento?> ObterPorIdAsync(int id)
        {
            return await dbContext.Medicamentos.FindAsync(id);
        }

        //Método que adiciona um novo medicamento ao banco de dados.
        public async Task<Medicamento> AdicionarAsync(Medicamento medicamento)
        {
            dbContext.Medicamentos.Add(medicamento);
            await dbContext.SaveChangesAsync();
            return medicamento;
        }

        //Método que atualiza as informãções de um medicamento existente.
        public async Task AtualizarAsync(Medicamento medicamento)
        {
            await dbContext.SaveChangesAsync();
        }


        //Método que remove um medicamento do banco de dados.
        public async Task RemoverAsync(Medicamento medicamento)
        {
            dbContext.Medicamentos.Remove(medicamento);
            await dbContext.SaveChangesAsync();
        }
    }
}