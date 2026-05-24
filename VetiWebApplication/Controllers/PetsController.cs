using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/pet")]
    public class PetsController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public PetsController(AppDbContext _dbContext) 
        { 
            dbContext = _dbContext; 
        }

        /// <summary>Lista todos os pets cadastrados.</summary>
        /// <remarks>Retorna todos os pets com os dados do tutor responsável.</remarks>
        /// <returns>Lista de pets.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pets = await dbContext.Pets.ToListAsync();
            return Ok(pets);
        }

        /// <summary>Busca um pet pelo ID.</summary>
        /// <param name="id">ID do pet.</param>
        /// <returns>Pet encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pet = await dbContext.Pets.Include(p => p.Tutor).FirstOrDefaultAsync(p => p.Id == id);
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
            var pets = await dbContext.Pets.Include(p => p.Tutor).Where(p => p.TutorId == tutorId).ToListAsync();
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
            var pets = await dbContext.Pets.Include(p => p.Tutor).Where(p => p.DsEspecie.ToLower() == especie.ToLower()).ToListAsync();
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
            if (string.IsNullOrEmpty(request.NmPet))
                return BadRequest("Nome do pet é obrigatório.");
            if (string.IsNullOrEmpty(request.DsEspecie))
                return BadRequest("Espécie é obrigatória.");
            if (string.IsNullOrEmpty(request.DsRaca))
                return BadRequest("Raça é obrigatória.");

            // Valida se o tutor existe
            var tutor = await dbContext.Tutores.FindAsync(request.TutorId);
            if (tutor == null)
                return NotFound($"Tutor com ID {request.TutorId} não encontrado.");

            var pet = new Pet
            {
                NmPet = request.NmPet,
                DsEspecie = request.DsEspecie,
                DsRaca = request.DsRaca,
                NrIdade = request.NrIdade,
                StCastrado = request.StCastrado,
                TutorId = request.TutorId
            };

            dbContext.Pets.Add(pet);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        }

        /// <summary>Atualiza os dados de um pet.</summary>
        /// <param name="id">ID do pet a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PetRequest petAtualizado)
        {
            var pet = await dbContext.Pets.FindAsync(id);
            if (pet == null) return NotFound("Pet não encontrado.");

            var tutor = await dbContext.Tutores.FindAsync(petAtualizado.TutorId);
            if (tutor == null) return NotFound($"Tutor com ID {petAtualizado.TutorId} não encontrado.");

            pet.NmPet = petAtualizado.NmPet;
            pet.DsEspecie = petAtualizado.DsEspecie;
            pet.DsRaca = petAtualizado.DsRaca;
            pet.NrIdade = petAtualizado.NrIdade;
            pet.StCastrado = petAtualizado.StCastrado;
            pet.TutorId = petAtualizado.TutorId;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Remove um pet pelo ID.</summary>
        /// <param name="id">ID do pet a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await dbContext.Pets.FindAsync(id);
            if (pet == null) return NotFound("Pet não encontrado.");

            dbContext.Pets.Remove(pet);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}