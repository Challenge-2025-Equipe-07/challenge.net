using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/exames")]
    public class ExamesController : ControllerBase
    {
        private readonly ExameService dbService;
        private readonly ILogger<ExamesController> dbLogger;
        public ExamesController(ExameService service, ILogger<ExamesController> logger) 
        {
            dbService = service;
            dbLogger = logger;
        }

        /// <summary>Lista todos os exames cadastrados.</summary>
        /// <returns>Lista de exames.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exames = await dbService.ObterTodosAsync();
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
            var exame = await dbService.ObterPorIdAsync(id);
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
            var (tutorExiste, exames) = await dbService.ObterPorTutorAsync(tutorId);
            if (!tutorExiste) return NotFound("Tutor não encontrado.");
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
            var exames = await dbService.ObterPorPetAsync(petId);
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
            var exames = await dbService.ObterPorConsultaAsync(consultaId);
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
            try
            {
                var exame = await dbService.CriarAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = exame.Id }, new
                {
                    exame.Id,
                    exame.DsDocumento,
                    exame.DtRealizacao,
                    exame.DsDiagnostico,
                    exame.ConsultaId
                });
            }
            catch (ArgumentException excecao)
            {
                dbLogger.LogError(excecao, "Erro ao criar exame: {Mensagem}", excecao.Message);
                return BadRequest(excecao.Message);
            }
            catch (KeyNotFoundException excecao)
            {
                return NotFound(excecao.Message);
            }
        }

        /// <summary>Atualiza os dados de um exame.</summary>
        /// <param name="id">ID do exame a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExameRequest exameAtualizado)
        {
            var sucesso = await dbService.AtualizarAsync(id, exameAtualizado);
            if (!sucesso) return NotFound("Exame não encontrado.");
            return NoContent();
        }

        /// <summary>Remove um exame pelo ID.</summary>
        /// <param name="id">ID do exame a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await dbService.RemoverAsync(id);
            if (!sucesso) return NotFound("Exame não encontrado.");
            return NoContent();
        }
    }
}