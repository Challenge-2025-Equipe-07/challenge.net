using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories
{
    // Implementação real, que lê e grava os dados de Tutor no banco Oracle.
    public class TutorRepository : ITutorRepository
    {

        // Contexto do banco de dados para acessar a tabela de Tutores.
        private readonly AppDbContext dbContext;

        public TutorRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }


        //Método para obter todos os tutores do banco de dados.
        public async Task<IEnumerable<Tutor>> ObterTodosAsync()
        {
            return await dbContext.Tutores.ToListAsync();
        }


        //Método para obter um tutor pelo seu ID, incluindo os pets associados.
        public async Task<Tutor?> ObterPorIdAsync(int id)
        {
            return await dbContext.Tutores.Include(t => t.Pets).FirstOrDefaultAsync(t => t.Id == id);
        }


        //Método para obter um tutor pelo seu CPF, incluindo os pets associados.
        public async Task<Tutor?> ObterPorCpfAsync(string cpf)
        {
            return await dbContext.Tutores.Include(t => t.Pets).FirstOrDefaultAsync(t => t.DsCpf == cpf);
        }


        //Método para adicionar um novo tutor ao banco de dados.
        public async Task<Tutor> AdicionarAsync(Tutor tutor)
        {
            dbContext.Tutores.Add(tutor);
            await dbContext.SaveChangesAsync();
            return tutor;
        }


        //Método para atualizar um tutor existente no banco de dados.
        public async Task AtualizarAsync(Tutor tutor)
        {
            await dbContext.SaveChangesAsync();
        }

        //Método para remover um tutor existente do banco de dados.
        public async Task RemoverAsync(Tutor tutor)
        {
            dbContext.Tutores.Remove(tutor);
            await dbContext.SaveChangesAsync();
        }
    }
}