using Microsoft.AspNetCore.Mvc;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/consulta")]
    public class ConsultasController : ControllerBase
    {
        private readonly ConsultaService dbService;
        private readonly ILogger<ConsultasController> dbLogger;
        public ConsultasController(ConsultaService service, ILogger<ConsultasController> logger) 
        {
            dbService = service;
            dbLogger = logger;
        }

        /// <summary>Lista todas as consultas cadastradas.</summary>
        /// <remarks>Retorna consultas com dados do pet e do veterinário.</remarks>
        /// <returns>Lista de consultas.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var consultas = await dbService.ObterTodasAsync();
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
            var consulta = await dbService.ObterPorIdAsync(id);
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
            var consultas = await dbService.ObterPorPetAsync(petId);
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
            try
            {
                var consulta = await dbService.CriarAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = consulta.Id }, consulta);
            }
            catch (ArgumentException excecao)
            {
                dbLogger.LogError(excecao, "Erro ao criar consulta: {Mensagem}", excecao.Message);
                return BadRequest(excecao.Message);
            }
            catch (KeyNotFoundException excecao)
            {
                return NotFound(excecao.Message);
            }
        }

        /// <summary>Atualiza os dados de uma consulta.</summary>
        /// <param name="id">ID da consulta a ser atualizada.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ConsultaRequest consultaAtualizada)
        {
            var (sucesso, vetNaoEncontrado) = await dbService.AtualizarAsync(id, consultaAtualizada);

            if (vetNaoEncontrado) return NotFound($"Veterinário com ID {consultaAtualizada.VeterinarioId} não encontrado.");
            if (!sucesso) return NotFound("Consulta não encontrada.");

            return NoContent();
        }

        /// <summary>Remove uma consulta pelo ID.</summary>
        /// <param name="id">ID da consulta a ser removida.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await dbService.RemoverAsync(id);
            if (!sucesso) return NotFound("Consulta não encontrada.");

            return NoContent();
        }
    }
}