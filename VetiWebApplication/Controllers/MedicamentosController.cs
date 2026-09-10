using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/medicamento")]
    public class MedicamentosController : ControllerBase
    {
        private readonly MedicamentoService dbService;
        private readonly ILogger<MedicamentosController> dbLogger;
        public MedicamentosController(MedicamentoService service, ILogger<MedicamentosController> logger) 
        {
            dbService = service;
            dbLogger = logger;
        }

        /// <summary>Lista todos os medicamentos cadastrados.</summary>
        /// <returns>Lista de medicamentos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var medicamentos = await dbService.ObterTodosAsync();
            return Ok(medicamentos.Select(m => new
            {
                m.Id,
                m.NmMedicamento,
                m.DsDosagem,
                m.DsFrequencia
            }));
        }

        /// <summary>Busca um medicamento pelo ID.</summary>
        /// <param name="id">ID do medicamento.</param>
        /// <returns>Medicamento encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medicamento = await dbService.ObterPorIdAsync(id);
            if (medicamento == null) return NotFound("Medicamento não encontrado.");

            return Ok(new
            {
                medicamento.Id,
                medicamento.NmMedicamento,
                medicamento.DsDosagem,
                medicamento.DsFrequencia
            });
        }

        /// <summary>Cadastra um novo medicamento.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/medicamento
        ///     {
        ///         "nmMedicamento": "Amoxicilina",
        ///         "dsDosagem": "500mg",
        ///         "dsFrequencia": "A cada 8 horas"
        ///     }
        /// </remarks>
        /// <returns>Medicamento criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicamentoRequest request)
        {
            try
            {
                var medicamento = await dbService.CriarAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = medicamento.Id }, new
                {
                    medicamento.Id,
                    medicamento.NmMedicamento,
                    medicamento.DsDosagem,
                    medicamento.DsFrequencia
                });
            }
            catch (ArgumentException excecao)
            {
                dbLogger.LogError(excecao, "Erro ao criar medicamento: {Mensagem}", excecao.Message);
                return BadRequest(excecao.Message);
            }
        }

        /// <summary>Atualiza os dados de um medicamento.</summary>
        /// <param name="id">ID do medicamento a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicamentoRequest request)
        {
            var sucesso = await dbService.AtualizarAsync(id, request);
            if (!sucesso) return NotFound("Medicamento não encontrado.");
            return NoContent();
        }

        /// <summary>Remove um medicamento pelo ID.</summary>
        /// <param name="id">ID do medicamento a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await dbService.RemoverAsync(id);
            if (!sucesso) return NotFound("Medicamento não encontrado.");
            return NoContent();
        }
    }
}