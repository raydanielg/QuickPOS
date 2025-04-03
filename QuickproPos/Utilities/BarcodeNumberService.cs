using System.Collections.Concurrent;

namespace QuickproPos.Utilities
{
    public class BarcodeNumberService
    {
        private static readonly ConcurrentDictionary<string, bool> GeneratedNumbers = new ConcurrentDictionary<string, bool>();

        public string GenerateBarcodeNumber()
        {
            string barcodeNumber;
            do
            {
                barcodeNumber = GenerateUniqueNumber();
            } while (!GeneratedNumbers.TryAdd(barcodeNumber, true));

            return barcodeNumber;
        }

        private string GenerateUniqueNumber()
        {
            // This is a simple example using DateTime ticks. You can replace this with a more sophisticated approach if needed.
            var ticks = DateTime.UtcNow.Ticks.ToString();
            return ticks;
        }
    }
}