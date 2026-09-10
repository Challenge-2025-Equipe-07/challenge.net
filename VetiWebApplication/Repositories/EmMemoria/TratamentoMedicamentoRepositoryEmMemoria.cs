using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories.EmMemoria
{
    //Classe de repositório de medicamentos vinculados
    //com tratamentos em memória para testes e desenvolvimento.
    public class TratamentoMedicamentoRepositoryEmMemoria : ITratamentoMedicamentoRepository
    {

        // Lista de medicamentos relacionados a tratamentos
        // armazenadas em memória.
        private readonly List<TratamentoMedicamento> dbTratamentoMedicamentos = new();


        //Método que verifica se a relação entre medicamento e tratamento existe
        public Task<bool> ExisteAsync(int tratamentoId, int medicamentoId)
        {
            return Task.FromResult(dbTratamentoMedicamentos
                .Any(tratamento => tratamento.TratamentoId == tratamentoId && tratamento.MedicamentoId == medicamentoId));
        }



        //Método que vincula um medicamento com um tratamento.
        public Task<TratamentoMedicamento> AdicionarAsync(TratamentoMedicamento tratamentoMedicamento)
        {
            dbTratamentoMedicamentos.Add(tratamentoMedicamento);
            return Task.FromResult(tratamentoMedicamento);
        }
    }
}