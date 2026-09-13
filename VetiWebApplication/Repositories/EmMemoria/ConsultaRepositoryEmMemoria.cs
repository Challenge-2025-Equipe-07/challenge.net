using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories.EmMemoria
{

    //Classe de repositório de Consultas em memória para testes e desenvolvimento.
    public class ConsultaRepositoryEmMemoria : IConsultaRepository
    {

        // Lista de consultas armazenadas em memória.
        private readonly List<Consulta> dbConsultas = new();


        // Contador para gerar IDs únicos para as consultas.
        private int dbProximoId = 1;

        // Simula o preenchimento dos objetos Pet e
        // Veterinario dentro da Consulta.
        private readonly IPetRepository dbPetRepository;
        private readonly IVeterinarioRepository dbVeterinarioRepository;

        public ConsultaRepositoryEmMemoria(IPetRepository petRepository, IVeterinarioRepository veterinarioRepository)
        {
            dbPetRepository = petRepository;
            dbVeterinarioRepository = veterinarioRepository;
        }


        //Método para obter todas as consultas armazenadas em memória
        public Task<IEnumerable<Consulta>> ObterTodasAsync()
        {
            return Task.FromResult<IEnumerable<Consulta>>(dbConsultas);
        }


        // Método para obter uma consulta pelo seu ID.
        public Task<Consulta?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(dbConsultas.FirstOrDefault(consulta => consulta.Id == id));
        }


        // Método para obter as consultas de um pet específico.
        public Task<IEnumerable<Consulta>> ObterPorPetAsync(int petId)
        {
            return Task.FromResult<IEnumerable<Consulta>>(dbConsultas.Where(consulta => consulta.PetId == petId).ToList());
        }


        //Método para adicionar uma nova consulta.
        public async Task<Consulta> AdicionarAsync(Consulta consulta)
        {
            consulta.Id = dbProximoId++;

            // preenche os objetos de navegação com base nos IDs, para que o Controller consiga acessar
            // consulta.Pet e consulta.Veterinario
            consulta.Pet = await dbPetRepository.ObterPorIdAsync(consulta.PetId);
            consulta.Veterinario = await dbVeterinarioRepository.ObterPorIdAsync(consulta.VeterinarioId);
            dbConsultas.Add(consulta);
            return consulta;
        }

        //Método para atualizar as informações de uma consulta.
        public Task AtualizarAsync(Consulta consulta)
        {
            return Task.CompletedTask;
        }


        //Método para remover uma consulta da lista de consultas armazendadas em memória.
        public Task RemoverAsync(Consulta consulta)
        {
            dbConsultas.Remove(consulta);
            return Task.CompletedTask;
        }

    }
}
