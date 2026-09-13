using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using VetiWebApplication.Interfaces;
using VetiWebApplication.Repositories.EmMemoria;

namespace VetiWebApplication.IntegrationTests
{
    
    // Substitui todos os repositórios Oracle por versões em memória.

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                SubstituirPorEmMemoria<ITutorRepository, TutorRepositoryEmMemoria>(services);
                SubstituirPorEmMemoria<IPetRepository, PetRepositoryEmMemoria>(services);
                SubstituirPorEmMemoria<IVeterinarioRepository, VeterinarioRepositoryEmMemoria>(services);
                SubstituirPorEmMemoria<IConsultaRepository, ConsultaRepositoryEmMemoria>(services);
                SubstituirPorEmMemoria<IExameRepository, ExameRepositoryEmMemoria>(services);
                SubstituirPorEmMemoria<IMedicamentoRepository, MedicamentoRepositoryEmMemoria>(services);
                SubstituirPorEmMemoria<IExameMedicamentoRepository, ExameMedicamentoRepositoryEmMemoria>(services);
                SubstituirPorEmMemoria<ITratamentoRepository, TratamentoRepositoryEmMemoria>(services);
                SubstituirPorEmMemoria<ITratamentoMedicamentoRepository, TratamentoMedicamentoRepositoryEmMemoria>(services);
            });
        }

        // Remove o registro real (Oracle)
        // e registra a versão em memória no lugar.
        private static void SubstituirPorEmMemoria<TInterface, TImplementacao>(IServiceCollection services)
            where TInterface : class
            where TImplementacao : class, TInterface
        {
            var descritor = services.SingleOrDefault(d => d.ServiceType == typeof(TInterface));
            if (descritor != null)
            {
                services.Remove(descritor);
            }

            services.AddSingleton<TInterface, TImplementacao>();
        }
    }
}