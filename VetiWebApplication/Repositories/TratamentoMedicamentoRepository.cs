using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories
{
    //Implementação real, que lê e grava os dados de medicamentos vinculados com tratamento no banco Oracle.
    public class TratamentoMedicamentoRepository : ITratamentoMedicamentoRepository
    {

        //Contexto do banco de dados para acessar a tabela de tratamento_medicamento.
        private readonly AppDbContext dbContext;

        public TratamentoMedicamentoRepository(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        //Método que verifica se a relação entre medicamento e tratamento existe
        public async Task<bool> ExisteAsync(int tratamentoId, int medicamentoId)
        {
            return await dbContext.TratamentoMedicamentos
                .AnyAsync(tratamento => tratamento.TratamentoId == tratamentoId && tratamento.MedicamentoId == medicamentoId);
        }


        //Método que vincula um medicamento com um tratamento.
        public async Task<TratamentoMedicamento> AdicionarAsync(TratamentoMedicamento tratamentoMedicamento)
        {
            dbContext.TratamentoMedicamentos.Add(tratamentoMedicamento);
            await dbContext.SaveChangesAsync();
            return tratamentoMedicamento;
        }
    }
}