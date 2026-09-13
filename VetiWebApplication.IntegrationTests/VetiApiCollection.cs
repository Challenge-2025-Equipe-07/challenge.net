namespace VetiWebApplication.IntegrationTests
{
    // Define uma "coleção" de testes que compartilham a mesma instância
    // do CustomWebApplicationFactory.
    [CollectionDefinition("Veti API")]
    public class VetiApiCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
   
    }
}