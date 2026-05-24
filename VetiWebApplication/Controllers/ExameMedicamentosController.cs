using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/exame-medicamento")]
    public class ExameMedicamentosController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public ExameMedicamentosController(AppDbContext _dbContext) 
        { 
            dbContext = _dbContext; 
        }

        /// <summary>Lista todos os medicamentos vinculados a exames.</summary>
        /// <returns>Lista de relações exame-medicamento.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = await dbContext.ExameMedicamentos
                .Include(em => em.Exame)
                .Include(em => em.Medicamento)
                .ToListAsync();

            return Ok(lista.Select(em => new
            {
                exame = new
                {
                    em.Exame.Id,
                    em.Exame.DsDocumento,
                    em.Exame.DsDiagnostico
                },
                medicamento = new
                {
                    em.Medicamento.Id,
                    em.Medicamento.NmMedicamento,
                    em.Medicamento.DsDosagem,
                    em.Medicamento.DsFrequencia
                },
                em.QtMedicamento
            }));
        }

        /// <summary>Lista medicamentos de um exame específico.</summary>
        /// <param name="exameId">ID do exame.</param>
        /// <returns>Lista de medicamentos do exame ou 404.</returns>
        [HttpGet("exame/{exameId}")]
        public async Task<IActionResult> GetByExame(int exameId)
        {
            var exame = await dbContext.Exames.FindAsync(exameId);
            if (exame == null) return NotFound("Exame não encontrado.");

            var lista = await dbContext.ExameMedicamentos
                .Include(em => em.Medicamento)
                .Where(em => em.ExameId == exameId)
                .ToListAsync();

            if (!lista.Any()) return NotFound("Nenhum medicamento encontrado para este exame.");

            return Ok(lista.Select(em => new
            {
                em.Medicamento.Id,
                em.Medicamento.NmMedicamento,
                em.Medicamento.DsDosagem,
                em.Medicamento.DsFrequencia,
                em.QtMedicamento
            }));
        }

        /// <summary>Lista exames que usaram um medicamento específico.</summary>
        /// <param name="medicamentoId">ID do medicamento.</param>
        /// <returns>Lista de exames ou 404.</returns>
        [HttpGet("medicamento/{medicamentoId}")]
        public async Task<IActionResult> GetByMedicamento(int medicamentoId)
        {
            var medicamento = await dbContext.Medicamentos.FindAsync(medicamentoId);
            if (medicamento == null) return NotFound("Medicamento não encontrado.");

            var lista = await dbContext.ExameMedicamentos
                .Include(em => em.Exame)
                .Where(em => em.MedicamentoId == medicamentoId)
                .ToListAsync();

            if (!lista.Any()) return NotFound("Nenhum exame encontrado para este medicamento.");

            return Ok(lista.Select(em => new
            {
                em.Exame.Id,
                em.Exame.DsDocumento,
                em.Exame.DsDiagnostico,
                em.Exame.DtRealizacao,
                em.QtMedicamento
            }));
        }

        /// <summary>Vincula um medicamento a um exame.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/exame-medicamento
        ///     {
        ///         "exameId": 1,
        ///         "medicamentoId": 1,
        ///         "qtMedicamento": 2
        ///     }
        /// </remarks>
        /// <returns>Relação criada com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExameMedicamentoRequest request)
        {
            var exame = await dbContext.Exames.FindAsync(request.ExameId);
            if (exame == null) return NotFound($"Exame com ID {request.ExameId} não encontrado.");

            var medicamento = await dbContext.Medicamentos.FindAsync(request.MedicamentoId);
            if (medicamento == null) return NotFound($"Medicamento com ID {request.MedicamentoId} não encontrado.");

            var jaExiste = await dbContext.ExameMedicamentos
                .AnyAsync(em => em.ExameId == request.ExameId && em.MedicamentoId == request.MedicamentoId);
            if (jaExiste) return BadRequest("Este medicamento já está vinculado a este exame.");

            var exameMedicamento = new ExameMedicamento
            {
                ExameId = request.ExameId,
                MedicamentoId = request.MedicamentoId,
                QtMedicamento = request.QtMedicamento
            };

            dbContext.ExameMedicamentos.Add(exameMedicamento);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByExame), new { exameId = request.ExameId }, new
            {
                request.ExameId,
                request.MedicamentoId,
                request.QtMedicamento
            });
        }

        /// <summary>Remove a relação entre um exame e um medicamento.</summary>
        /// <param name="exameId">ID do exame.</param>
        /// <param name="medicamentoId">ID do medicamento.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{exameId}/{medicamentoId}")]
        public async Task<IActionResult> Delete(int exameId, int medicamentoId)
        {
            var exameMedicamento = await dbContext.ExameMedicamentos
                .FirstOrDefaultAsync(em => em.ExameId == exameId && em.MedicamentoId == medicamentoId);

            if (exameMedicamento == null) return NotFound("Relação não encontrada.");

            dbContext.ExameMedicamentos.Remove(exameMedicamento);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}