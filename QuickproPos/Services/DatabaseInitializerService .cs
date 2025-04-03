using QuickproPos.Data;

namespace QuickproPos.Services
{
    public class DatabaseInitializerService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var dbContext = new DatabaseContext();

            try
            {
                await dbContext.InitializeAsync();
                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during database initialization: {ex.Message}");
            }
            finally
            {
                await dbContext.DisposeAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}