using Microsoft.AspNetCore.Mvc;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/exame-medicamento")]
    public class ExameMedicamentosController : ControllerBase
    {
        private readonly ExameMedicamentoService dbService;
        private readonly ILogger<ExameMedicamentosController> dbLogger;
        public ExameMedicamentosController(ExameMedicamentoService service, ILogger<ExameMedicamentosController> logger) 
        {
            dbService = service;
            dbLogger = logger;
        }

        /// <summary>Lista todos os medicamentos vinculados a exames.</summary>
        /// <returns>Lista de relações exame-medicamento.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = await dbService.ObterTodosAsync();
          
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
            var (exameExiste, lista) = await dbService.ObterPorExameAsync(exameId);
            if (!exameExiste) return NotFound("Exame não encontrado.");
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
            var (medicamentoExiste, lista) = await dbService.ObterPorMedicamentoAsync(medicamentoId);
            if (!medicamentoExiste) return NotFound("Medicamento não encontrado.");
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
            try
            {
                var criado = await dbService.CriarAsync(request);
                return CreatedAtAction(nameof(GetByExame), new { exameId = request.ExameId }, new
                {
                    request.ExameId,
                    request.MedicamentoId,
                    request.QtMedicamento
                });
            }
            catch (KeyNotFoundException excecao)
            {
                return NotFound(excecao.Message);
            }
            catch (InvalidOperationException excecao)
            {
                dbLogger.LogWarning(excecao, "Erro ao vincular medicamento a exame.");
                return BadRequest(excecao.Message);
            }
        }

        /// <summary>Remove a relação entre um exame e um medicamento.</summary>
        /// <param name="exameId">ID do exame.</param>
        /// <param name="medicamentoId">ID do medicamento.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{exameId}/{medicamentoId}")]
        public async Task<IActionResult> Delete(int exameId, int medicamentoId)
        {
            var sucesso = await dbService.RemoverAsync(exameId, medicamentoId);
            if (!sucesso) return NotFound("Relação não encontrada.");
            return NoContent();
        }
    }
}