using VetiWebApplication.Models;
using VetiWebApplication.Interfaces;

namespace VetiWebApplication.Repositories.EmMemoria
{

    //Classe de repositório de Exames em memória para testes e desenvolvimento.
    public class ExameRepositoryEmMemoria : IExameRepository
    {
        // Lista de exames armazenados em memória.
        private readonly List<Exame> dbExames = new();

        // Contador para gerar IDs únicos para os exames.
        private int dbProximoId = 1;

        // Simula o preenchimento do objeto Consulta dentro do Exame.
        private readonly IConsultaRepository dbConsultaRepository;
        public ExameRepositoryEmMemoria(IConsultaRepository consultaRepository)
        {
            dbConsultaRepository = consultaRepository;
        }

        //Método para obter todos os exames armazenados em memória.
        public Task<IEnumerable<Exame>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<Exame>>(dbExames);
        }

        //Método para obter um exame de ID específico.
        public Task<Exame?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(dbExames.FirstOrDefault(exame => exame.Id == id));
        }


        //Método para obter exames de uma consulta específica por seu ID.
        public Task<IEnumerable<Exame>> ObterPorConsultaAsync(int consultaId)
        {
            return Task.FromResult<IEnumerable<Exame>>(dbExames.Where(exame => exame.ConsultaId == consultaId).ToList());
        }


        //Método para obter os exames de um pet específico.
        public Task<IEnumerable<Exame>> ObterPorPetAsync(int petId)
        {
            return Task.FromResult<IEnumerable<Exame>>(
                dbExames.Where(exame => exame.Consulta != null && exame.Consulta.Pet != null && exame.Consulta.Pet.Id == petId).ToList());
        }


        //Método para adicionar um novo exame.
        public async Task<Exame> AdicionarAsync(Exame exame)
        {
            exame.Id = dbProximoId++;

            // preenche a Consulta, que já vem com Pet e Veterinario preenchidos, graças ao
            // ConsultaRepositoryEmMemoria.
            exame.Consulta = await dbConsultaRepository.ObterPorIdAsync(exame.ConsultaId);
            dbExames.Add(exame);
            return exame;
        }

        //Método para atualizar as informações de um exame.
        public Task AtualizarAsync(Exame exame)
        {
            return Task.CompletedTask;
        }

        //Método para remover um exame da lista de exames armazenados.
        public Task RemoverAsync(Exame exame)
        {
            dbExames.Remove(exame);
            return Task.CompletedTask;
        }
    }
}
