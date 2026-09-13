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


        //Simula o preenchimento do Tratamento (incluindo sua lista de medicamentos)
        // e o Medicamento dentro da relação TratamentoMedicamento.
        private readonly ITratamentoRepository dbTratamentoRepository;
        private readonly IMedicamentoRepository dbMedicamentoRepository;

        public TratamentoMedicamentoRepositoryEmMemoria(
            ITratamentoRepository tratamentoRepository,
            IMedicamentoRepository medicamentoRepository)
        {
            dbTratamentoRepository = tratamentoRepository;
            dbMedicamentoRepository = medicamentoRepository;
        }


        //Método que verifica se a relação entre medicamento e tratamento existe
        public Task<bool> ExisteAsync(int tratamentoId, int medicamentoId)
        {
            return Task.FromResult(dbTratamentoMedicamentos
                .Any(tratamento => tratamento.TratamentoId == tratamentoId && tratamento.MedicamentoId == medicamentoId));
        }



        //Método que vincula um medicamento com um tratamento.
        public async Task<TratamentoMedicamento> AdicionarAsync(TratamentoMedicamento tratamentoMedicamento)
        {

            var tratamento = await dbTratamentoRepository.ObterPorIdAsync(tratamentoMedicamento.TratamentoId);
            var medicamento = await dbMedicamentoRepository.ObterPorIdAsync(tratamentoMedicamento.MedicamentoId);

            tratamentoMedicamento.Tratamento = tratamento;
            tratamentoMedicamento.Medicamento = medicamento;
            dbTratamentoMedicamentos.Add(tratamentoMedicamento);

            //adiciona essa relação na lista de medicamentos
            // do próprio Tratamento, para que t.TratamentoMedicamentos reflita
            // o vínculo recém-criado.
            tratamento?.TratamentoMedicamentos?.Add(tratamentoMedicamento);
            return tratamentoMedicamento;
        }
    }
}