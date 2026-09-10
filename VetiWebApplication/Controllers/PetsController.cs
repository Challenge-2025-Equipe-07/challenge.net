using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models.Requests;
using VetiWebApplication.Services;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/pet")]
    public class PetsController : ControllerBase
    {

        // Declara o serviço responsável pelas operações relacionadas aos pets.
        private readonly PetService dbService;

        // Declara o logger utilizado para registrar informações e erros do controller.
        private readonly ILogger<PetsController> dbLogger;

        // Construtor que recebe o serviço de pets e o logger por injeção de dependência.
        public PetsController(PetService service, ILogger<PetsController> logger) 
        { 
            dbService = service;
            dbLogger = logger;
        }

        /// <summary>Lista todos os pets cadastrados.</summary>
        /// <remarks>Retorna todos os pets com os dados do tutor responsável.</remarks>
        /// <returns>Lista de pets.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pets = await dbService.ObterTodosAsync();
            return Ok(pets);
        }

        /// <summary>Busca um pet pelo ID.</summary>
        /// <param name="id">ID do pet.</param>
        /// <returns>Pet encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pet = await dbService.ObterPorIdAsync(id);
            if (pet == null) return NotFound("Pet não encontrado.");
            return Ok(new
            {
                pet.Id,
                pet.NmPet,
                pet.DsEspecie,
                pet.DsRaca,
                pet.NrIdade,
                pet.StCastrado,
                tutor = new
                {
                    pet.Tutor.Id,
                    pet.Tutor.NmTutor,
                    pet.Tutor.DsEmail,
                    pet.Tutor.DsTelefone
                }
            });
        }

        /// <summary>Lista todos os pets de um tutor.</summary>
        /// <param name="tutorId">ID do tutor.</param>
        /// <returns>Lista de pets do tutor ou 404.</returns>
        [HttpGet("tutor/{tutorId}")]
        public async Task<IActionResult> GetByTutor(int tutorId)
        {
            var pets = await dbService.ObterPorTutorAsync(tutorId);
            if (!pets.Any()) return NotFound("Nenhum pet encontrado para este tutor.");
            return Ok(pets);
        }

        /// <summary>Filtra pets por espécie.</summary>
        /// <remarks>Exemplo: cachorro, gato, pássaro.</remarks>
        /// <param name="especie">Espécie do pet.</param>
        /// <returns>Lista de pets da espécie informada ou 404.</returns>
        [HttpGet("especie/{especie}")]
        public async Task<IActionResult> GetByEspecie(string especie)
        {
            var pets = await dbService.ObterPorEspecieAsync(especie);
            if (!pets.Any()) return NotFound("Nenhum pet encontrado para esta espécie.");
            return Ok(pets.Select(p => new
            {
                p.Id,
                p.NmPet,
                p.DsEspecie,
                p.DsRaca,
                p.NrIdade,
                p.StCastrado,
                tutor = new
                {
                    p.Tutor.Id,
                    p.Tutor.NmTutor,
                    p.Tutor.DsEmail
                }
            }));
        }

        /// <summary>Cadastra um novo pet.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/pet
        ///     {
        ///         "nmPet": "Sauro",
        ///         "dsEspecie": "Gato",
        ///         "dsRaca": "SRD",
        ///         "nrIdade": 6,
        ///         "stCastrado": 1,
        ///         "tutorId": 1
        ///     }
        ///
        /// stCastrado: 1 = castrado, 0 = não castrado
        /// </remarks>
        /// <returns>Pet criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PetRequest request)
        {
            try
            {
                var pet = await dbService.CriarAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
            }
            catch (ArgumentException excecao)
            {
                dbLogger.LogError(excecao, "Erro ao criar pet: {Mensagem}", excecao.Message);
                return BadRequest(excecao.Message);
            }
            catch (KeyNotFoundException excecao)
            {
           
                return NotFound(excecao.Message);
            }
        }

        /// <summary>Atualiza os dados de um pet.</summary>
        /// <param name="id">ID do pet a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PetRequest petAtualizado)
        {
            var (sucesso, tutorNaoEncontrado) = await dbService.AtualizarAsync(id, petAtualizado);

            if (tutorNaoEncontrado) return NotFound($"Tutor com ID {petAtualizado.TutorId} não encontrado.");
            if (!sucesso) return NotFound("Pet não encontrado.");

            return NoContent();
        }

        /// <summary>Remove um pet pelo ID.</summary>
        /// <param name="id">ID do pet a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await dbService.RemoverAsync(id);
            if (!sucesso) return NotFound("Pet não encontrado.");
            return NoContent();
        }
    }
}