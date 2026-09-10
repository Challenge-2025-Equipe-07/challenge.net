using Microsoft.Extensions.Diagnostics.HealthChecks;
using VetiWebApplication.Data;

namespace VetiWebApplication.HealthChecks
{
    // Verifica se a API consegue realmente se conectar ao banco Oracle.
    public class OracleHealthCheck : IHealthCheck
    {
        private readonly AppDbContext dbContext;

        public OracleHealthCheck(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext _dbContext,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Tenta abrir uma conexão real com o Oracle.
                var conseguiuConectar = await dbContext.Database.CanConnectAsync(cancellationToken);

                return conseguiuConectar
                    ? HealthCheckResult.Healthy("Conexão com o Oracle está funcionando.")
                    : HealthCheckResult.Unhealthy("Não foi possível conectar ao Oracle.");
            }
            catch (Exception excecao)
            {
                return HealthCheckResult.Unhealthy("Erro ao conectar ao Oracle.", excecao);
            }
        }
    }
}