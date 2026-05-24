using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/exames")]
    public class ExamesController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public ExamesController(AppDbContext _dbContext) 
        { 
            dbContext = _dbContext; 
        }

        /// <summary>Lista todos os exames cadastrados.</summary>
        /// <returns>Lista de exames.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exames = await dbContext.Exames.Include(e => e.Consulta).ToListAsync();
            return Ok(exames.Select(e => new
            {
                e.Id,
                e.DsDocumento,
                e.DtRealizacao,
                e.DsDiagnostico,
                consulta = new
                {
                    e.Consulta.Id,
                    e.Consulta.DtConsulta,
                    e.Consulta.TpEvento,
              
                }
            }));
        }

        /// <summary>Busca um exame pelo ID.</summary>
        /// <param name="id">ID do exame.</param>
        /// <returns>Exame encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exame = await dbContext.Exames.Include(e => e.Consulta).FirstOrDefaultAsync(e => e.Id == id);
            if (exame == null) return NotFound("Exame não encontrado.");
            return Ok(new
            {
                exame.Id,
                exame.DsDocumento,
                exame.DtRealizacao,
                exame.DsDiagnostico,
                consulta = new
                {
                    exame.Consulta.Id,
                    exame.Consulta.DtConsulta,
                    exame.Consulta.TpEvento,
                
                }
            });
        }

            /// <summary>Lista todos os exames de um tutor.</summary>
            /// <remarks>Retorna todos os exames de todos os pets vinculados ao tutor.</remarks>
            /// <param name="tutorId">ID do tutor.</param>
            /// <returns>Lista de exames do tutor ou 404.</returns>
            [HttpGet("tutor/{tutorId}")]
        public async Task<IActionResult> GetByTutor(int tutorId)
        {
            // Valida se o tutor existe
            var tutor = await dbContext.Tutores.FindAsync(tutorId);
            if (tutor == null) return NotFound("Tutor não encontrado.");

            var exames = await dbContext.Exames
                .Include(e => e.Consulta).ThenInclude(c => c.Pet)
                .Where(e => e.Consulta.Pet.TutorId == tutorId)
                .ToListAsync();

            if (!exames.Any()) return NotFound("Nenhum exame encontrado para este tutor.");
            return Ok(exames.Select(e => new
            {
                e.Id,
                e.DsDocumento,
                e.DtRealizacao,
                e.DsDiagnostico,
                consulta = new
                {
                    e.Consulta.Id,
                    e.Consulta.DtConsulta,
                    e.Consulta.TpEvento,
                    pet = new
                    {
                        e.Consulta.Pet.Id,
                        e.Consulta.Pet.NmPet
                    }
                }
            }));
        }

        /// <summary>Lista todos os exames de um pet.</summary>
        /// <remarks>Retorna todos os exames realizados pelo pet informado.</remarks>
        /// <param name="petId">ID do pet.</param>
        /// <returns>Lista de exames do pet ou 404.</returns>
        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetByPet(int petId)
        {
            // Valida se o pet existe
            var pet = await dbContext.Pets.FindAsync(petId);
            if (pet == null) return NotFound("Pet não encontrado.");
          
            var exames = await dbContext.Exames
                .Include(e => e.Consulta).ThenInclude(c => c.Pet)
                .Where(e => e.Consulta.Pet.Id == petId)
                .ToListAsync();

            if (!exames.Any()) return NotFound("Nenhum exame encontrado para este pet.");
            return Ok(exames.Select(e => new
            {
                e.Id,
                e.DsDocumento,
                e.DtRealizacao,
                e.DsDiagnostico,
                e.ConsultaId
            }));
        }

        /// <summary>Lista exames de uma consulta específica.</summary>
        /// <param name="consultaId">ID da consulta.</param>
        /// <returns>Lista de exames da consulta ou 404.</returns>
        [HttpGet("consulta/{consultaId}")]
        public async Task<IActionResult> GetByConsulta(int consultaId)
        {
            var exames = await dbContext.Exames.Where(e => e.ConsultaId == consultaId).ToListAsync();
            if (!exames.Any()) return NotFound("Nenhum exame encontrado para esta consulta.");

            return Ok(exames.Select(e => new
            {
                e.Id,
                e.DsDocumento,
                e.DtRealizacao,
                e.DsDiagnostico
            }));
        }

        /// <summary>Cadastra um novo exame.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/exames
        ///     {
        ///         "dsDocumento": "hemograma.pdf",
        ///         "dtRealizacao": "2026-05-20",
        ///         "dsDiagnostico": "Anemia leve",
        ///         "consultaId": 1
        ///     }
        /// </remarks>
        /// <returns>Exame criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExameRequest request)
        {
            if (string.IsNullOrEmpty(request.DsDocumento))
                return BadRequest("Documento é obrigatório.");
            if (string.IsNullOrEmpty(request.DsDiagnostico))
                return BadRequest("Diagnóstico é obrigatório.");

            // Valida se a consulta existe
            var consulta = await dbContext.Consultas.FindAsync(request.ConsultaId);
            if (consulta == null)
                return NotFound($"Consulta com ID {request.ConsultaId} não encontrada.");

            var exame = new Exame
            {
                DsDocumento = request.DsDocumento,
                DtRealizacao = request.DtRealizacao,
                DsDiagnostico = request.DsDiagnostico,
                ConsultaId = request.ConsultaId
            };

            dbContext.Exames.Add(exame);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = exame.Id }, new
            {
                exame.Id,
                exame.DsDocumento,
                exame.DtRealizacao,
                exame.DsDiagnostico,
                exame.ConsultaId
            });
        }

        /// <summary>Atualiza os dados de um exame.</summary>
        /// <param name="id">ID do exame a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExameRequest exameAtualizado)
        {
            var exame = await dbContext.Exames.FindAsync(id);
            if (exame == null) return NotFound("Exame não encontrado.");

            exame.DsDocumento = exameAtualizado.DsDocumento;
            exame.DtRealizacao = exameAtualizado.DtRealizacao;
            exame.DsDiagnostico = exameAtualizado.DsDiagnostico;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Remove um exame pelo ID.</summary>
        /// <param name="id">ID do exame a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exame = await dbContext.Exames.FindAsync(id);
            if (exame == null) return NotFound("Exame não encontrado.");

            dbContext.Exames.Remove(exame);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}