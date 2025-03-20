using System.Text.Json;
using big_hw_1.models;

namespace big_hw_1.visitors
{
    public class JsonFileExporterVisitor : IFileExporterVisitor
    {
        private readonly Dictionary<string, object> _data = new();

        public void Visit(IEnumerable<BankAccount> bankAccounts) => _data["BankAccounts"] = bankAccounts;
        public void Visit(IEnumerable<Category> categories) => _data["Categories"] = categories;
        public void Visit(IEnumerable<Operation> operations) => _data["Operations"] = operations;

        public void PushDataToFile(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(_data, options));
            Console.WriteLine($"Data written to {filePath}");
        }
    }
}
