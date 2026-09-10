using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace VetiWebApplication.Repositories
{

    // Implementação real, que lê e grava os dados de Exame no banco Oracle.
    public class ExameRepository : IExameRepository
    {
        // Contexto do banco de dados para acessar a tabela de Exames.
        private readonly AppDbContext dbContext;

        public ExameRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        //Método que obtem todos os exames do banco de dados.
        public async Task<IEnumerable<Exame>> ObterTodosAsync()
        {
            return await dbContext.Exames.Include(exame => exame.Consulta).ToListAsync();
        }
        
        //Método que obtem um exame específico por seu ID.
        public async Task<Exame?> ObterPorIdAsync(int id)
        {
            return await dbContext.Exames.Include(exame => exame.Consulta).FirstOrDefaultAsync(exame => exame.Id == id);
        }

        //Método para obter um exame pelo ID de uma consulta específica
        public async Task<IEnumerable<Exame>> ObterPorConsultaAsync(int consultaId)
        {
            return await dbContext.Exames.Where(exame => exame.ConsultaId == consultaId).ToListAsync();
        }


        //Método para obter um exame pelo ID de um pet específico
        public async Task<IEnumerable<Exame>> ObterPorPetAsync(int petId)
        {
            return await dbContext.Exames
                .Include(exame => exame.Consulta).ThenInclude(consulta => consulta.Pet)
                .Where(exame => exame.Consulta.Pet.Id == petId)
                .ToListAsync();
        }


        //Método para adicionar um novo exame ao banco de dados.
        public async Task<Exame> AdicionarAsync(Exame exame)
        {
            dbContext.Exames.Add(exame);
            await dbContext.SaveChangesAsync();
            return exame;
        }

        //Método para atualizar informações de um exame existente.
        public async Task AtualizarAsync(Exame exame)
        {
            await dbContext.SaveChangesAsync();
        }


        //Método para remover um exame do banco de dados.
        public async Task RemoverAsync(Exame exame)
        {
            dbContext.Exames.Remove(exame);
            await dbContext.SaveChangesAsync();
        }
    }
}
