using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;

namespace VetiWebApplication.Repositories.EmMemoria
{

    //Classe de repositório de Pets em memória para testes e desenvolvimento..
    public class TutorRepositoryEmMemoria : ITutorRepository
    {

        // Lista de tutores armazenados em memória.
        private readonly List<Tutor> dbTutores = new();

        // Contador para gerar IDs únicos para os tutores.
        private int dbProximoId = 1;


        //Método para obter todos os tutores armazenados em memória.
        public Task<IEnumerable<Tutor>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<Tutor>>(dbTutores);
        }


        //Método para obter um tutor pelo seu ID.
        public Task<Tutor?> ObterPorIdAsync(int id)
        {
            return Task.FromResult(dbTutores.FirstOrDefault(tutor => tutor.Id == id));
        }


        //Método para obter um tutor pelo seu CPF.
        public Task<Tutor?> ObterPorCpfAsync(string cpf)
        {
            return Task.FromResult(dbTutores.FirstOrDefault(tutor => tutor.DsCpf == cpf));
        }


        //Método para obter um tutor pelo seu e-mail.
        public Task<Tutor> AdicionarAsync(Tutor tutor)
        {
            tutor.Id = dbProximoId++;
            dbTutores.Add(tutor);
            return Task.FromResult(tutor);
        }


        //Método para atualizar um tutor existente.
        public Task AtualizarAsync(Tutor tutor)
        {
            // Como o objeto já está na lista por referência, não precisa fazer nada extra.
            return Task.CompletedTask;
        }


        //Método para remover um tutor existente.
        public Task RemoverAsync(Tutor tutor)
        {
            dbTutores.Remove(tutor);
            return Task.CompletedTask;
        }
    }
}