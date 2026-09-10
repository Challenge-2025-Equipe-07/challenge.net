using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories.EmMemoria
{
    //Classe de repositório de Medicamentos em memória para testes e desenvolvimento.
    public class MedicamentoRepositoryEmMemoria : IMedicamentoRepository
    {

        // Lista de medicamentos armazenados em memória.
        private readonly List<Medicamento> dbMedicamentos = new();
       
        // Contador para gerar IDs únicos para os medicamentos.
        private int dbProximoId = 1;


        //Método que obtem todos os medicamentos do banco de dados.
        public Task<IEnumerable<Medicamento>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<Medicamento>>(dbMedicamentos);
        }


        //Método que obtem um medicameto específico pelo seu ID.
        public Task<Medicamento?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(dbMedicamentos.FirstOrDefault(medicamento => medicamento.Id == id));
        }

        //Método que adiciona um novo medicamento ao banco de dados.
        public Task<Medicamento> AdicionarAsync(Medicamento medicamento)
        {
            medicamento.Id = dbProximoId++;
            dbMedicamentos.Add(medicamento);
            return Task.FromResult(medicamento);
        }


        //Método que atualiza as informãções de um medicamento existente.
        public Task AtualizarAsync(Medicamento medicamento)
        {
            return Task.CompletedTask;
        }

        //Método que remove um medicamento do banco de dados.
        public Task RemoverAsync(Medicamento medicamento)
        {
            dbMedicamentos.Remove(medicamento);
            return Task.CompletedTask;
        }
    }
}
