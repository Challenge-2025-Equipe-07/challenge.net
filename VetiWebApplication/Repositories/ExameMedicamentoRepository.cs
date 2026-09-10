using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories
{
    // Implementação real, que lê e grava os dados de medicamentos de exames no banco Oracle.
    public class ExameMedicamentoRepository : IExameMedicamentoRepository
    {
        //Contexto do banco de dados para acessar a tabela de ExameMedicamentos.
        private readonly AppDbContext dbContext;

        public ExameMedicamentoRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }


        //Método para obter todos os medicamentos e exames relacionados.
        public async Task<IEnumerable<ExameMedicamento>> ObterTodosAsync()
        {
            return await dbContext.ExameMedicamentos
                .Include(em => em.Exame)
                .Include(em => em.Medicamento)
                .ToListAsync();
        }

        //Método para obter medicamentos relacionados a um exame específico
        public async Task<IEnumerable<ExameMedicamento>> ObterPorExameAsync(int exameId)
        {
            return await dbContext.ExameMedicamentos
                .Include(em => em.Medicamento)
                .Where(em => em.ExameId == exameId)
                .ToListAsync();
        }

        //Método para obter exames relacionados a um medicamento específico.
        public async Task<IEnumerable<ExameMedicamento>> ObterPorMedicamentoAsync(int medicamentoId)
        {
            return await dbContext.ExameMedicamentos
                .Include(em => em.Exame)
                .Where(em => em.MedicamentoId == medicamentoId)
                .ToListAsync();
        }

        //Método para obter uma relação específica pelo ExameId + MedicamentoId.
        public async Task<ExameMedicamento?> ObterPorChaveAsync(int exameId, int medicamentoId)
        {
            return await dbContext.ExameMedicamentos
                .FirstOrDefaultAsync(em => em.ExameId == exameId && em.MedicamentoId == medicamentoId);
        }


        //Método que obtem se existe uma relação entre um exame e um medicamento específicos
        public async Task<bool> ExisteAsync(int exameId, int medicamentoId)
        {
            return await dbContext.ExameMedicamentos
                .AnyAsync(em => em.ExameId == exameId && em.MedicamentoId == medicamentoId);
        }

        //Método para vincular um medicamento a um exame.
        public async Task<ExameMedicamento> AdicionarAsync(ExameMedicamento exameMedicamento)
        {
            dbContext.ExameMedicamentos.Add(exameMedicamento);
            await dbContext.SaveChangesAsync();
            return exameMedicamento;
        }

        //Método para remover uma relação existente.
        public async Task RemoverAsync(ExameMedicamento exameMedicamento)
        {
            dbContext.ExameMedicamentos.Remove(exameMedicamento);
            await dbContext.SaveChangesAsync();
        }
    }
}