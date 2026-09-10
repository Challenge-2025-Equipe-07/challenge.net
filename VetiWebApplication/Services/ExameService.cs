using VetiWebApplication.Interfaces;
using VetiWebApplication.Models;
using VetiWebApplication.Models.Requests;

namespace VetiWebApplication.Services
{
    // Camada de regra de negócio para Exames.
    public class ExameService
    {

        // Repositório de exames para operações de CRUD.
        private readonly IExameRepository dbRepository;

        // Repositório de consultas para validar a existência da consulta ao criar ou atualizar uma exame.
        private readonly IConsultaRepository dbConsultaRepository;

        // Repositório de tutor para validar a existência de um tutor ao criar ou atualizar uma exame.
        private readonly ITutorRepository dbTutorRepository;

        // Logger para registrar eventos importantes.
        private readonly ILogger<ExameService> dbLogger;

        public ExameService(
            IExameRepository repository,
            IConsultaRepository consultaRepository,
            ITutorRepository tutorRepository,
            ILogger<ExameService> logger)
        {
            dbRepository = repository;
            dbConsultaRepository = consultaRepository;
            dbTutorRepository = tutorRepository;
            dbLogger = logger;
        }

        //Método para obter todos os exames.
        public async Task<IEnumerable<Exame>> ObterTodosAsync()
        {
            return await dbRepository.ObterTodosAsync();
        }

        //Método para obter um exame de ID específico.
        public async Task<Exame?> ObterPorIdAsync(int id)
        {
            return await dbRepository.ObterPorIdAsync(id);
        }


        //Método para obter exames de uma consulta específica por seu ID.
        public async Task<IEnumerable<Exame>> ObterPorConsultaAsync(int consultaId)
        {
            return await dbRepository.ObterPorConsultaAsync(consultaId);
        }


        //Método para obter os exames de um pet específico.
        public async Task<IEnumerable<Exame>> ObterPorPetAsync(int petId)
        {
            return await dbRepository.ObterPorPetAsync(petId);
        }

       //Método para obter os exames relacionados a um tutor específico.
        public async Task<(bool tutorExiste, IEnumerable<Exame> exames)> ObterPorTutorAsync(int tutorId)
        {
            var tutor = await dbTutorRepository.ObterPorIdAsync(tutorId);
            if (tutor == null) return (false, Enumerable.Empty<Exame>());

            var todos = await dbRepository.ObterTodosAsync();
            var exames = todos.Where(exame => exame.Consulta != null && exame.Consulta.Pet != null && exame.Consulta.Pet.TutorId == tutorId);
            return (true, exames);
        }

        //Método para adicionar um novo exame.
        public async Task<Exame> CriarAsync(ExameRequest request)
        {
            //Documento é obrigatório
            if (string.IsNullOrEmpty(request.DsDocumento))
            {
                dbLogger.LogWarning("Tentativa de criar exame sem documento.");
                throw new ArgumentException("Documento é obrigatório.");
            }

            //Diagnóstico é obrigatório

            if (string.IsNullOrEmpty(request.DsDiagnostico))
            {
                dbLogger.LogWarning("Tentativa de criar exame sem diagnóstico.");
                throw new ArgumentException("Diagnóstico é obrigatório.");
            }

            //Valida se a consulta existe
            var consulta = await dbConsultaRepository.ObterPorIdAsync(request.ConsultaId);
            if (consulta == null)
            {
                dbLogger.LogWarning("Tentativa de criar exame com ConsultaId inexistente: {ConsultaId}", request.ConsultaId);
                throw new KeyNotFoundException($"Consulta com ID {request.ConsultaId} não encontrada.");
            }

            var exame = new Exame
            {
                DsDocumento = request.DsDocumento,
                DtRealizacao = request.DtRealizacao,
                DsDiagnostico = request.DsDiagnostico,
                ConsultaId = request.ConsultaId
            };

            var exameCriado = await dbRepository.AdicionarAsync(exame);

            dbLogger.LogInformation("Novo exame criado: {Documento}", exameCriado.DsDocumento);

            return exameCriado;
        }

        //Método para atualizar as informações de um exame.
        public async Task<bool> AtualizarAsync(int id, ExameRequest request)
        {
            var exame = await dbRepository.ObterPorIdAsync(id);
            if (exame == null) return false;

            exame.DsDocumento = request.DsDocumento;
            exame.DtRealizacao = request.DtRealizacao;
            exame.DsDiagnostico = request.DsDiagnostico;

            await dbRepository.AtualizarAsync(exame);
            return true;
        }


        //Método para remover um exame da lista de exames armazenados.
        public async Task<bool> RemoverAsync(int id)
        {
            var exame = await dbRepository.ObterPorIdAsync(id);
            if (exame == null) return false;

            await dbRepository.RemoverAsync(exame);
            return true;
        }
    }
}