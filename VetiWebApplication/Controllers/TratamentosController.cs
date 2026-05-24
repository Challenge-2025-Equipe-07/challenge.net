using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/tratamento")]
    public class TratamentosController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public TratamentosController(AppDbContext _dbContext) 
        { 
            dbContext = _dbContext; 
        }

        /// <summary>Lista todos os tratamentos cadastrados.</summary>
        /// <returns>Lista de tratamentos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tratamentos = await dbContext.Tratamentos
                .Include(t => t.Pet)
                .Include(t => t.TratamentoMedicamentos).ThenInclude(tm => tm.Medicamento)
                .ToListAsync();

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
            var tratamento = await dbContext.Tratamentos
                .Include(t => t.Pet)
                .Include(t => t.TratamentoMedicamentos).ThenInclude(tm => tm.Medicamento)
                .FirstOrDefaultAsync(t => t.Id == id);

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
            var pet = await dbContext.Pets.FindAsync(petId);
            if (pet == null) return NotFound("Pet não encontrado.");

            var tratamentos = await dbContext.Tratamentos
                .Include(t => t.TratamentoMedicamentos).ThenInclude(tm => tm.Medicamento)
                .Where(t => t.PetId == petId)
                .ToListAsync();

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
            if (string.IsNullOrEmpty(request.DsDiagnostico))
                return BadRequest("Diagnóstico é obrigatório.");

            var pet = await dbContext.Pets.FindAsync(request.PetId);
            if (pet == null) return NotFound($"Pet com ID {request.PetId} não encontrado.");

            var tratamento = new Tratamento
            {
                DsDiagnostico = request.DsDiagnostico,
                DtInicio = request.DtInicio,
                DtRetornoPrevisto = request.DtRetornoPrevisto,
                DsObservacao = request.DsObservacao,
                PetId = request.PetId
            };

            dbContext.Tratamentos.Add(tratamento);
            await dbContext.SaveChangesAsync();

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
            var tratamento = await dbContext.Tratamentos.FindAsync(tratamentoId);
            if (tratamento == null) return NotFound("Tratamento não encontrado.");

            var medicamento = await dbContext.Medicamentos.FindAsync(request.MedicamentoId);
            if (medicamento == null) return NotFound($"Medicamento com ID {request.MedicamentoId} não encontrado.");

            var jaExiste = await dbContext.TratamentoMedicamentos
                .AnyAsync(tm => tm.TratamentoId == tratamentoId && tm.MedicamentoId == request.MedicamentoId);
            if (jaExiste) return BadRequest("Este medicamento já está vinculado a este tratamento.");

            var tratamentoMedicamento = new TratamentoMedicamento
            {
                TratamentoId = tratamentoId,
                MedicamentoId = request.MedicamentoId,
                QtMedicamento = request.QtMedicamento,
                DsInstrucao = request.DsInstrucao
            };

            dbContext.TratamentoMedicamentos.Add(tratamentoMedicamento);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = tratamentoId }, new
            {
                tratamentoId,
                request.MedicamentoId,
                request.QtMedicamento,
                request.DsInstrucao
            });
        }

        /// <summary>Atualiza um tratamento.</summary>
        /// <param name="id">ID do tratamento a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TratamentoRequest request)
        {
            var tratamento = await dbContext.Tratamentos.FindAsync(id);
            if (tratamento == null) return NotFound("Tratamento não encontrado.");

            tratamento.DsDiagnostico = request.DsDiagnostico;
            tratamento.DtInicio = request.DtInicio;
            tratamento.DtRetornoPrevisto = request.DtRetornoPrevisto;
            tratamento.DsObservacao = request.DsObservacao;
            tratamento.PetId = request.PetId;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Remove um tratamento pelo ID.</summary>
        /// <param name="id">ID do tratamento a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tratamento = await dbContext.Tratamentos.FindAsync(id);
            if (tratamento == null) return NotFound("Tratamento não encontrado.");

            dbContext.Tratamentos.Remove(tratamento);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}