using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/medicamento")]
    public class MedicamentosController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public MedicamentosController(AppDbContext _dbContext) { dbContext = _dbContext; }

        /// <summary>Lista todos os medicamentos cadastrados.</summary>
        /// <returns>Lista de medicamentos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var medicamentos = await dbContext.Medicamentos.ToListAsync();
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
            var medicamento = await dbContext.Medicamentos.FindAsync(id);
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
            if (string.IsNullOrEmpty(request.NmMedicamento))
                return BadRequest("Nome do medicamento é obrigatório.");
            if (string.IsNullOrEmpty(request.DsDosagem))
                return BadRequest("Dosagem é obrigatória.");
            if (string.IsNullOrEmpty(request.DsFrequencia))
                return BadRequest("Frequência é obrigatória.");

            var medicamento = new Medicamento
            {
                NmMedicamento = request.NmMedicamento,
                DsDosagem = request.DsDosagem,
                DsFrequencia = request.DsFrequencia
            };

            dbContext.Medicamentos.Add(medicamento);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = medicamento.Id }, new
            {
                medicamento.Id,
                medicamento.NmMedicamento,
                medicamento.DsDosagem,
                medicamento.DsFrequencia
            });
        }

        /// <summary>Atualiza os dados de um medicamento.</summary>
        /// <param name="id">ID do medicamento a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicamentoRequest request)
        {
            var medicamento = await dbContext.Medicamentos.FindAsync(id);
            if (medicamento == null) return NotFound("Medicamento não encontrado.");

            medicamento.NmMedicamento = request.NmMedicamento;
            medicamento.DsDosagem = request.DsDosagem;
            medicamento.DsFrequencia = request.DsFrequencia;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Remove um medicamento pelo ID.</summary>
        /// <param name="id">ID do medicamento a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var medicamento = await dbContext.Medicamentos.FindAsync(id);
            if (medicamento == null) return NotFound("Medicamento não encontrado.");

            dbContext.Medicamentos.Remove(medicamento);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}