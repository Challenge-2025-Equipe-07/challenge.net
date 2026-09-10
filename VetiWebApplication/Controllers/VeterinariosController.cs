using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/veterinario")]
    public class VeterinariosController : ControllerBase
    {


        // Declara o serviço responsável pelas operações relacionadas aos veterinários.
        private readonly VeterinarioService dbService;

        // Declara o logger utilizado para registrar informações e erros do controller.
        private readonly ILogger<VeterinariosController> dbLogger;

        // Construtor que recebe o serviço de veterinários e o logger por injeção de dependência.
        public VeterinariosController(VeterinarioService service, ILogger<VeterinariosController> logger) 
        { 
            dbService = service;
            dbLogger = logger;
        }

        /// <summary>Lista todos os veterinários cadastrados.</summary>
        /// <returns>Lista de veterinários.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vets = await dbService.ObterTodosAsync();
            return Ok(vets);
        }

        /// <summary>Busca um veterinário pelo ID.</summary>
        /// <param name="id">ID do veterinário.</param>
        /// <returns>Veterinário encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vet = await dbService.ObterPorIdAsync(id);
            if (vet == null) return NotFound("Veterinário não encontrado.");
            return Ok(vet);
        }

        /// <summary>Cadastra um novo veterinário.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/veterinario
        ///     {
        ///         "dsEmail": "vet@clinica.com",
        ///         "dsPassword": "123456"
        ///     }
        /// </remarks>
        /// <returns>Veterinário criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeterinarioRequest request)
        {
            try
            {
                var vet = await dbService.CriarAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = vet.Id }, vet);
            }
            catch (ArgumentException excecao)
            {
                dbLogger.LogError(excecao, "Erro ao criar veterinário: {Mensagem}", excecao.Message);
                return BadRequest(excecao.Message);
            }
        }

        /// <summary>Atualiza os dados de um veterinário.</summary>
        /// <param name="id">ID do veterinário a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VeterinarioRequest vetAtualizado)
        {
            var sucesso = await dbService.AtualizarAsync(id, vetAtualizado);
            if (!sucesso) return NotFound("Veterinário não encontrado.");
            return NoContent();
        }

        /// <summary>Remove um veterinário pelo ID.</summary>
        /// <param name="id">ID do veterinário a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await dbService.RemoverAsync(id);
            if (!sucesso) return NotFound("Veterinário não encontrado.");
            return NoContent();
        }
    }
}