using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Data;
using VetiWebApplication.Models;

namespace VetiWebApplication.Controllers
{
    [ApiController]
    [Route("api/tutor")]
    public class TutoresController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public TutoresController(AppDbContext _dbContext) 
        { 
            dbContext = _dbContext; 
        }

        /// <summary>Lista todos os tutores cadastrados.</summary>
        /// <remarks>Retorna todos os tutores com seus respectivos pets.</remarks>
        /// <returns>Lista de tutores.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tutores = await dbContext.Tutores.ToListAsync();
            return Ok(tutores);
        }

        /// <summary>Busca um tutor pelo ID.</summary>
        /// <remarks>Retorna o tutor com seus pets cadastrados.</remarks>
        /// <param name="id">ID do tutor.</param>
        /// <returns>Tutor encontrado ou 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tutor = await dbContext.Tutores.Include(t => t.Pets).FirstOrDefaultAsync(t => t.Id == id);
            if (tutor == null) return NotFound("Tutor não encontrado.");
            return Ok(new
            {
                tutor.Id,
                tutor.NmTutor,
                tutor.DsCpf,
                tutor.DsEmail,
                tutor.DsTelefone,
                pets = tutor.Pets.Select(p => new
                {
                    p.Id,
                    p.NmPet,
                    p.DsEspecie,
                    p.DsRaca,
                    p.NrIdade,
                    p.StCastrado
                })
            });
        }

        /// <summary>Busca um tutor pelo CPF.</summary>
        /// <remarks>O CPF deve ser informado sem pontos ou traços.</remarks>
        /// <param name="cpf">CPF do tutor.</param>
        /// <returns>Tutor encontrado ou 404.</returns>
        [HttpGet("cpf/{cpf}")]
        public async Task<IActionResult> GetByCpf(string cpf)
        {
            var tutor = await dbContext.Tutores.Include(t => t.Pets).FirstOrDefaultAsync(t => t.DsCpf == cpf);
            if (tutor == null) return NotFound("Tutor não encontrado.");
            return Ok(tutor);
        }


        /// <summary>Cadastra um novo tutor.</summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/tutor
        ///     {
        ///         "nmTutor": "Laura Lopes",
        ///         "dsCpf": "12345678901",
        ///         "dsEmail": "laura@email.com",
        ///         "dsTelefone": "11999999999"
        ///     }
        /// </remarks>
        /// <returns>Tutor criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TutorRequest request)
        {
            if (string.IsNullOrEmpty(request.NmTutor))
                return BadRequest("Nome é obrigatório.");
            if (string.IsNullOrEmpty(request.DsCpf))
                return BadRequest("CPF é obrigatório.");
            if (string.IsNullOrEmpty(request.DsTelefone))
                return BadRequest("Telefone é obrigatório.");

            var tutor = new Tutor
            {
                NmTutor = request.NmTutor,
                DsCpf = request.DsCpf,
                DsEmail = request.DsEmail,
                DsTelefone = request.DsTelefone
            };

            dbContext.Tutores.Add(tutor);
            await dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = tutor.Id }, tutor);
        }

        /// <summary>Atualiza os dados de um tutor.</summary>
        /// <remarks>Todos os campos serão substituídos pelos novos valores informados.</remarks>
        /// <param name="id">ID do tutor a ser atualizado.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TutorRequest tutorAtualizado)
        {
            var tutor = await dbContext.Tutores.FindAsync(id);
            if (tutor == null) return NotFound("Tutor não encontrado.");

            tutor.NmTutor = tutorAtualizado.NmTutor;
            tutor.DsCpf = tutorAtualizado.DsCpf;
            tutor.DsEmail = tutorAtualizado.DsEmail;
            tutor.DsTelefone = tutorAtualizado.DsTelefone;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Remove um tutor pelo ID.</summary>
        /// <remarks>A remoção é permanente e não pode ser desfeita.</remarks>
        /// <param name="id">ID do tutor a ser removido.</param>
        /// <returns>204 em caso de sucesso ou 404 se não encontrado.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tutor = await dbContext.Tutores.FindAsync(id);
            if (tutor == null) return NotFound("Tutor não encontrado.");

            dbContext.Tutores.Remove(tutor);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}