using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories.EmMemoria
{
    //Classe de repositório de Tratamentos em memória para testes e desenvolvimento.
    public class TratamentoRepositoryEmMemoria : ITratamentoRepository
    {

        // Lista de tratamenrtos armazenados em memória.
        private readonly List<Tratamento> dbTratamentos = new();

        // Contador para gerar IDs únicos para os tratamentos.
        private int dbProximoId = 1;


        //Simula o preenchimento do objeto Pet dentro do Tratamento.
        private readonly IPetRepository dbPetRepository;

        public TratamentoRepositoryEmMemoria(IPetRepository petRepository)
        {
            dbPetRepository = petRepository;
        }

        //Método para obter todos os tratamentos do banco de dados
        public Task<IEnumerable<Tratamento>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<Tratamento>>(dbTratamentos);
        }


        //Método para obter um tratamento específico.
        public Task<Tratamento?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(dbTratamentos.FirstOrDefault(t => t.Id == id));
        }


        //Método para obter o tratamento de um pet específico.
        public Task<IEnumerable<Tratamento>> ObterPorPetAsync(int petId)
        {
            return Task.FromResult<IEnumerable<Tratamento>>(dbTratamentos.Where(t => t.PetId == petId).ToList());
        }

        //Método para adicionar um novo tratamento ao banco de dados
        public async Task<Tratamento> AdicionarAsync(Tratamento tratamento)
        {
            tratamento.Id = dbProximoId++;
            // Simula o preenchimento do Pet com base no PetId.
            tratamento.Pet = await dbPetRepository.ObterPorIdAsync(tratamento.PetId);

            // Inicializa a lista de medicamentos vazia, para evitar erro de
            // referência nula quando o Controller tentar fazer .Select() nela.
            tratamento.TratamentoMedicamentos = new List<TratamentoMedicamento>();

            dbTratamentos.Add(tratamento);
            return tratamento;
        }


        //Método para atualizar as informações de um tratamento existente no banco de dados
        public Task AtualizarAsync(Tratamento tratamento)
        {
            return Task.CompletedTask;
        }

        //Método para remover um tratamento do banco de dados.
        public Task RemoverAsync(Tratamento tratamento)
        {
            dbTratamentos.Remove(tratamento);
            return Task.CompletedTask;
        }
    }
}