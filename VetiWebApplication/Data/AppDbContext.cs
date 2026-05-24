using Microsoft.EntityFrameworkCore;
using VetiWebApplication.Models;

namespace VetiWebApplication.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Veterinario> Veterinarios { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<Exame> Exames { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<ExameMedicamento> ExameMedicamentos { get; set; }
        public DbSet<Tratamento> Tratamentos { get; set; }
        public DbSet<TratamentoMedicamento> TratamentoMedicamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nomes das tabelas no Oracle
            modelBuilder.Entity<Tutor>().ToTable("TB_TUTOR");
            modelBuilder.Entity<Veterinario>().ToTable("TB_VETERINARIO");
            modelBuilder.Entity<Pet>().ToTable("TB_PET");
            modelBuilder.Entity<Consulta>().ToTable("TB_CONSULTA");
            modelBuilder.Entity<Exame>().ToTable("TB_EXAME");
            modelBuilder.Entity<Medicamento>().ToTable("TB_MEDICAMENTO");
            modelBuilder.Entity<ExameMedicamento>().ToTable("TB_EXAME_MEDICAMENTO");
            modelBuilder.Entity<Tratamento>().ToTable("TB_TRATAMENTO");
            modelBuilder.Entity<TratamentoMedicamento>().ToTable("TB_TRATAMENTO_MEDICAMENTO");

            // Chave composta da tabela de relação
            modelBuilder.Entity<TratamentoMedicamento>()
                .HasKey(tm => new { tm.TratamentoId, tm.MedicamentoId });

            // Chave composta da tabela de relação
            modelBuilder.Entity<ExameMedicamento>()
                .HasKey(em => new { em.ExameId, em.MedicamentoId });

            // CPF e Email do tutor são únicos
            modelBuilder.Entity<Tutor>()
                .HasIndex(t => t.DsCpf).IsUnique();
            modelBuilder.Entity<Tutor>()
                .HasIndex(t => t.DsEmail).IsUnique();

            // Email do veterinário é único
            modelBuilder.Entity<Veterinario>()
                .HasIndex(v => v.DsEmail).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}