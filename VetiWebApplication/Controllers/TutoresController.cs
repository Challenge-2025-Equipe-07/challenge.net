using Microsoft.AspNetCore.Mvc;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/tutor")]
    public class TutoresController : ControllerBase
    {

        // Declara o serviço responsável pelas operações relacionadas aos tutores
        private readonly TutorService dbService;

        // Declara o logger utilizado para registrar informações e erros do controller
        private readonly ILogger<TutoresController> dbLogger;

        // Construtor que recebe o serviço de tutores e o logger por injeção de dependência
        public TutoresController(TutorService service, ILogger<TutoresController> logger) 
        {
            dbService = service;
            dbLogger = logger;
        }

        /// <summary>Lista todos os tutores cadastrados.</summary>
        /// <remarks>Retorna todos os tutores com seus respectivos pets.</remarks>
        /// <returns>Lista de tutores.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tutores = await dbService.ObterTodosAsync();
            return Ok(tutores);
        }

        /// <summary>Busca um tutor pelo ID.</summary>
        /// <remarks>Retorna o tutor com seus pets cadastrados.</remarks>
        /// <param name="id">ID do tutor.</param>
        /// <returns>Tutor encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tutor = await dbService.ObterPorIdAsync(id);
            if (tutor == null) return NotFound("Tutor não encontrado.");
            return Ok(new
            {
                tutor.Id,
                tutor.NmTutor,
                tutor.DsCpf,
                tutor.DsEmail,
                tutor.DsTelefone,
                pets = tutor.Pets.Select(p => new
                {
                    p.Id,
                    p.NmPet,
                    p.DsEspecie,
                    p.DsRaca,
                    p.NrIdade,
                    p.StCastrado
                })
            });
        }

        /// <summary>Busca um tutor pelo CPF.</summary>
        /// <remarks>O CPF deve ser informado sem pontos ou traços.</remarks>
        /// <param name="cpf">CPF do tutor.</param>
        /// <returns>Tutor encontrado ou 404.</returns>
        [HttpGet("cpf/{cpf}")]
        public async Task<IActionResult> GetByCpf(string cpf)
        {
            var tutor = await dbService.ObterPorCpfAsync(cpf);
            if (tutor == null) return NotFound("Tutor não encontrado.");
            return Ok(tutor);
        }


        /// <summary>Cadastra um novo tutor.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/tutor
        ///     {
        ///         "nmTutor": "Laura Lopes",
        ///         "dsCpf": "12345678901",
        ///         "dsEmail": "laura@email.com",
        ///         "dsTelefone": "11999999999"
        ///     }
        /// </remarks>
        /// <returns>Tutor criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TutorRequest request)
        {
            try
            {
                var tutor = await dbService.CriarAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = tutor.Id }, tutor);
            }
            catch (ArgumentException excecao)
            {
                dbLogger.LogError(excecao, "Erro ao criar tutor: {Mensagem}", excecao.Message);
                return BadRequest(excecao.Message);
            }
            
        }

        /// <summary>Atualiza os dados de um tutor.</summary>
        /// <remarks>Todos os campos serão substituídos pelos novos valores informados.</remarks>
        /// <param name="id">ID do tutor a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TutorRequest tutorAtualizado)
        {
            var sucesso = await dbService.AtualizarAsync(id, tutorAtualizado);
            if (!sucesso) return NotFound("Tutor não encontrado.");
            return NoContent();
        }

        /// <summary>Remove um tutor pelo ID.</summary>
        /// <remarks>A remoção é permanente e não pode ser desfeita.</remarks>
        /// <param name="id">ID do tutor a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await dbService.RemoverAsync(id);
            if (!sucesso) return NotFound("Tutor não encontrado.");
            return NoContent();
        }
    }
}