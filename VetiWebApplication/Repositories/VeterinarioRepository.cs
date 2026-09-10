using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories
{

    // Implementação real, que lê e grava os dados de Veterinário no banco Oracle.
    public class VeterinarioRepository : IVeterinarioRepository
    {

        // Contexto do banco de dados para acessar a tabela de Veterinários.
        private readonly AppDbContext dbContext;

        public VeterinarioRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }


        //Método para obter todos os veterinários do banco de dados.
        public async Task<IEnumerable<Veterinario>> ObterTodosAsync()
        {
            return await dbContext.Veterinarios.ToListAsync();
        }


        //Método para obter um veterinário específico pelo seu ID.
        public async Task<Veterinario?> ObterPorIdAsync(int id)
        {
            return await dbContext.Veterinarios.FindAsync(id);
        }

        //Método para adicionar um novo veterinário ao banco de dados.
        public async Task<Veterinario> AdicionarAsync(Veterinario veterinario)
        {
            dbContext.Veterinarios.Add(veterinario);
            await dbContext.SaveChangesAsync();
            return veterinario;
        }


        //Método para atualizar um veterinário existente no banco de dados.
        public async Task AtualizarAsync(Veterinario veterinario)
        {
            await dbContext.SaveChangesAsync();
        }

        //Método para remover um veterinário existente do banco de dados.
        public async Task RemoverAsync(Veterinario veterinario)
        {
            dbContext.Veterinarios.Remove(veterinario);
            await dbContext.SaveChangesAsync();
        }
    }
}