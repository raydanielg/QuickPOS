using QuickproPos.Data;
using QuickproPos.Data.Setting;

namespace QuickproPos.Services
{
    public class SeedDataService
    {
        private readonly DatabaseContext _context;
        public SeedDataService(DatabaseContext context)
        {
            _context = context;
        }
        public async Task SeedDataAsync()
        {

        }
    }
}
