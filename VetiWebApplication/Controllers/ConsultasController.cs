using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/consulta")]
    public class ConsultasController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public ConsultasController(AppDbContext _dbContext) 
        { 
            dbContext = _dbContext; 
        }

        /// <summary>Lista todas as consultas cadastradas.</summary>
        /// <remarks>Retorna consultas com dados do pet e do veterinário.</remarks>
        /// <returns>Lista de consultas.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var consultas = await dbContext.Consultas
                .Include(c => c.Pet)
                .Include(c => c.Veterinario).ToListAsync();
            return Ok(consultas.Select(c => new
            {
                c.Id,
                c.DtConsulta,
                c.TpEvento,
                c.Notificar,
                pet = new
                {
                    c.Pet.Id,
                    c.Pet.NmPet,
                    c.Pet.DsEspecie
                },
                veterinario = new
                {
                    c.Veterinario.Id,
                    c.Veterinario.DsEmail
                }
            }));
        }

        /// <summary>Busca uma consulta pelo ID.</summary>
        /// <param name="id">ID da consulta.</param>
        /// <returns>Consulta encontrada ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var consulta = await dbContext.Consultas
                .Include(c => c.Pet).ThenInclude(p => p.Tutor)
                .Include(c => c.Veterinario)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (consulta == null) return NotFound("Consulta não encontrada.");
            return Ok(new
            {
                consulta.Id,
                consulta.DtConsulta,
                consulta.TpEvento,
                consulta.Notificar,
                pet = new
                {
                    consulta.Pet.Id,
                    consulta.Pet.NmPet,
                    consulta.Pet.DsEspecie,
                    tutor = new
                    {
                        consulta.Pet.Tutor.Id,
                        consulta.Pet.Tutor.NmTutor,
                        consulta.Pet.Tutor.DsEmail
                    }
                },
                veterinario = new
                {
                    consulta.Veterinario.Id,
                    consulta.Veterinario.DsEmail
                }
            });
        }

        /// <summary>Lista o histórico de consultas de um pet.</summary>
        /// <param name="petId">ID do pet.</param>
        /// <returns>Lista de consultas do pet ou 404.</returns>
        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetByPet(int petId)
        {
            var consultas = await dbContext.Consultas
                .Include(c => c.Pet)
                .Include(c => c.Veterinario)
                .Where(c => c.PetId == petId)
                .ToListAsync();
            if (!consultas.Any()) return NotFound("Nenhuma consulta encontrada para este pet.");
            return Ok(consultas.Select(c => new
            {
                c.Id,
                c.DtConsulta,
                c.TpEvento,
                c.Notificar,
                veterinario = new
                {
                    c.Veterinario.Id,
                    c.Veterinario.DsEmail
                }
            }));
        }

       
        /// <summary>Registra uma nova consulta.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/appointment
        ///     {
        ///         "dtConsulta": "2026-05-20T10:00:00",
        ///         "tpEvento": "Consulta",
        ///         "notificar": 1,
        ///         "petId": 1,
        ///         "veterinarioId": 1
        ///     }
        ///
        /// notificar: 1 = sim, 0 = não
        /// tpEvento: Consulta de rotina, Consulta de Emergência, Retorno, Medicação
        /// </remarks>
        /// <returns>Consulta criada com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConsultaRequest request)
        {
            if (string.IsNullOrEmpty(request.TpEvento))
                return BadRequest("Tipo do evento é obrigatório.");

            // Valida se o pet existe
            var pet = await dbContext.Pets.FindAsync(request.PetId);
            if (pet == null)
                return NotFound($"Pet com ID {request.PetId} não encontrado.");

            // Valida se o veterinário existe
            var vet = await dbContext.Veterinarios.FindAsync(request.VeterinarioId);
            if (vet == null)
                return NotFound($"Veterinário com ID {request.VeterinarioId} não encontrado.");

            var consulta = new Consulta
            {
                DtConsulta = request.DtConsulta,
                TpEvento = request.TpEvento,
                Notificar = request.Notificar,
                PetId = request.PetId,
                VeterinarioId = request.VeterinarioId
            };

            dbContext.Consultas.Add(consulta);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = consulta.Id }, consulta);
        }

        /// <summary>Atualiza os dados de uma consulta.</summary>
        /// <param name="id">ID da consulta a ser atualizada.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ConsultaRequest consultaAtualizada)
        {
            var consulta = await dbContext.Consultas.FindAsync(id);
            if (consulta == null) return NotFound("Consulta não encontrada.");

            var vet = await dbContext.Veterinarios.FindAsync(consultaAtualizada.VeterinarioId);
            if (vet == null) return NotFound($"Veterinário com ID {consultaAtualizada.VeterinarioId} não encontrado.");

            consulta.DtConsulta = consultaAtualizada.DtConsulta;
            consulta.TpEvento = consultaAtualizada.TpEvento;
            consulta.Notificar = consultaAtualizada.Notificar;
            consulta.PetId = consultaAtualizada.PetId;
            consulta.VeterinarioId = consultaAtualizada.VeterinarioId;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Remove uma consulta pelo ID.</summary>
        /// <param name="id">ID da consulta a ser removida.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var consulta = await dbContext.Consultas.FindAsync(id);
            if (consulta == null) return NotFound("Consulta não encontrada.");

            dbContext.Consultas.Remove(consulta);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}