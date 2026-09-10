using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/tratamento")]
    public class TratamentosController : ControllerBase
    {
        private readonly TratamentoService dbService;
        private readonly ILogger<TratamentosController> dbLogger;

        public TratamentosController(TratamentoService service, ILogger<TratamentosController> logger) 
        {
            dbService = service;
            dbLogger = logger;
        }

        /// <summary>Lista todos os tratamentos cadastrados.</summary>
        /// <returns>Lista de tratamentos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tratamentos = await dbService.ObterTodosAsync();
            return Ok(tratamentos.Select(t => new
            {
                t.Id,
                t.DsDiagnostico,
                t.DtInicio,
                t.DtRetornoPrevisto,
                t.DsObservacao,
                pet = new
                {
                    t.Pet.Id,
                    t.Pet.NmPet,
                    t.Pet.DsEspecie
                },
                medicamentos = t.TratamentoMedicamentos.Select(tm => new
                {
                    tm.Medicamento.Id,
                    tm.Medicamento.NmMedicamento,
                    tm.Medicamento.DsDosagem,
                    tm.Medicamento.DsFrequencia,
                    tm.QtMedicamento,
                    tm.DsInstrucao
                })
            }));
        }

        /// <summary>Busca um tratamento pelo ID.</summary>
        /// <param name="id">ID do tratamento.</param>
        /// <returns>Tratamento encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tratamento = await dbService.ObterPorIdAsync(id);
            if (tratamento == null) return NotFound("Tratamento não encontrado.");

            return Ok(new
            {
                tratamento.Id,
                tratamento.DsDiagnostico,
                tratamento.DtInicio,
                tratamento.DtRetornoPrevisto,
                tratamento.DsObservacao,
                pet = new
                {
                    tratamento.Pet.Id,
                    tratamento.Pet.NmPet,
                    tratamento.Pet.DsEspecie
                },
                medicamentos = tratamento.TratamentoMedicamentos.Select(tm => new
                {
                    tm.Medicamento.Id,
                    tm.Medicamento.NmMedicamento,
                    tm.Medicamento.DsDosagem,
                    tm.Medicamento.DsFrequencia,
                    tm.QtMedicamento,
                    tm.DsInstrucao
                })
            });
        }

        /// <summary>Lista tratamentos de um pet.</summary>
        /// <param name="petId">ID do pet.</param>
        /// <returns>Lista de tratamentos do pet ou 404.</returns>
        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetByPet(int petId)
        {
            var (petExiste, tratamentos) = await dbService.ObterPorPetAsync(petId);
            if (!petExiste) return NotFound("Pet não encontrado.");
            if (!tratamentos.Any()) return NotFound("Nenhum tratamento encontrado para este pet.");

            return Ok(tratamentos.Select(t => new
            {
                t.Id,
                t.DsDiagnostico,
                t.DtInicio,
                t.DtRetornoPrevisto,
                t.DsObservacao,
                medicamentos = t.TratamentoMedicamentos.Select(tm => new
                {
                    tm.Medicamento.NmMedicamento,
                    tm.Medicamento.DsDosagem,
                    tm.Medicamento.DsFrequencia,
                    tm.QtMedicamento,
                    tm.DsInstrucao
                })
            }));
        }

        /// <summary>Cadastra um novo tratamento.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/tratamento
        ///     {
        ///         "dsDiagnostico": "Infecção bacteriana",
        ///         "dtInicio": "2026-05-24",
        ///         "dtRetornoPrevisto": "2026-06-07",
        ///         "dsObservacao": "Manter em repouso",
        ///         "petId": 1
        ///     }
        /// </remarks>
        /// <returns>Tratamento criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TratamentoRequest request)
        {
            try
            {
                var tratamento = await dbService.CriarAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = tratamento.Id }, new
                {
                    tratamento.Id,
                    tratamento.DsDiagnostico,
                    tratamento.DtInicio,
                    tratamento.DtRetornoPrevisto,
                    tratamento.DsObservacao,
                    tratamento.PetId
                });
            }
            catch (ArgumentException excecao)
            {
                dbLogger.LogError(excecao, "Erro ao criar tratamento: {Mensagem}", excecao.Message);
                return BadRequest(excecao.Message);
            }
            catch (KeyNotFoundException excecao)
            {
                return NotFound(excecao.Message);
            }
        }

        /// <summary>Adiciona um medicamento a um tratamento.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/tratamento/1/medicamento
        ///     {
        ///         "medicamentoId": 1,
        ///         "qtMedicamento": 2,
        ///         "dsInstrucao": "Tomar após refeição"
        ///     }
        /// </remarks>
        /// <returns>201 em caso de sucesso.</returns>
        [HttpPost("{tratamentoId}/medicamento")]
        public async Task<IActionResult> AddMedicamento(int tratamentoId, [FromBody] TratamentoMedicamentoRequest request)
        {
            try
            {
                var criado = await dbService.AdicionarMedicamentoAsync(tratamentoId, request);
                return CreatedAtAction(nameof(GetById), new { id = tratamentoId }, new
                {
                    tratamentoId,
                    request.MedicamentoId,
                    request.QtMedicamento,
                    request.DsInstrucao
                });
            }
            catch (KeyNotFoundException excecao)
            {
                return NotFound(excecao.Message);
            }
            catch (InvalidOperationException excecao)
            {
                dbLogger.LogWarning(excecao, "Erro ao adicionar medicamento a tratamento.");
                return BadRequest(excecao.Message);
            }
        }

            /// <summary>Atualiza um tratamento.</summary>
            /// <param name="id">ID do tratamento a ser atualizado.</param>
            /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
            [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TratamentoRequest request)
        {
            var sucesso = await dbService.AtualizarAsync(id, request);
            if (!sucesso) return NotFound("Tratamento não encontrado.");
            return NoContent();
        }

        /// <summary>Remove um tratamento pelo ID.</summary>
        /// <param name="id">ID do tratamento a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await dbService.RemoverAsync(id);
            if (!sucesso) return NotFound("Tratamento não encontrado.");
            return NoContent();
        }
    }
}