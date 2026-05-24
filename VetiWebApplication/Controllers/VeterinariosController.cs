using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/veterinario")]
    public class VeterinariosController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public VeterinariosController(AppDbContext _dbContext) 
        { 
            dbContext = _dbContext; 
        }

        /// <summary>Lista todos os veterinários cadastrados.</summary>
        /// <returns>Lista de veterinários.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vets = await dbContext.Veterinarios.ToListAsync();
            return Ok(vets);
        }

        /// <summary>Busca um veterinário pelo ID.</summary>
        /// <param name="id">ID do veterinário.</param>
        /// <returns>Veterinário encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vet = await dbContext.Veterinarios.FindAsync(id);
            if (vet == null) return NotFound("Veterinário não encontrado.");
            return Ok(vet);
        }

        /// <summary>Cadastra um novo veterinário.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/veterinario
        ///     {
        ///         "dsEmail": "vet@clinica.com",
        ///         "dsPassword": "123456"
        ///     }
        /// </remarks>
        /// <returns>Veterinário criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeterinarioRequest request)
        {
            if (string.IsNullOrEmpty(request.DsEmail))
                return BadRequest("Email é obrigatório.");
            if (string.IsNullOrEmpty(request.DsPassword))
                return BadRequest("Senha é obrigatória.");

            var vet = new Veterinario
            {
                DsEmail = request.DsEmail,
                DsPassword = request.DsPassword
            };

            dbContext.Veterinarios.Add(vet);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = vet.Id }, vet);
        }

        /// <summary>Atualiza os dados de um veterinário.</summary>
        /// <param name="id">ID do veterinário a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VeterinarioRequest vetAtualizado)
        {
            var vet = await dbContext.Veterinarios.FindAsync(id);
            if (vet == null) return NotFound("Veterinário não encontrado.");

            vet.DsEmail = vetAtualizado.DsEmail;
            vet.DsPassword = vetAtualizado.DsPassword;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Remove um veterinário pelo ID.</summary>
        /// <param name="id">ID do veterinário a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vet = await dbContext.Veterinarios.FindAsync(id);
            if (vet == null) return NotFound("Veterinário não encontrado.");

            dbContext.Veterinarios.Remove(vet);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}