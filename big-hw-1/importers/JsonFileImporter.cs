using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using big_hw_1.models;
using big_hw_1.facades;

namespace big_hw_1.importers
{
    public class JsonFileImporter : FileImporter
    {
        public JsonFileImporter(BankAccountFacade bankAccountFacade, 
                                CategoryFacade categoryFacade, 
                                OperationFacade operationFacade) 
            : base(bankAccountFacade, categoryFacade, operationFacade) {
            
        }

        protected override void ReadAndParseFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("File not found", filePath);
                }

                string jsonContent = File.ReadAllText(filePath);
                
                var importedData = JsonSerializer.Deserialize<ImportedData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (importedData == null)
                {
                    throw new Exception("Failed to parse JSON.");
                }

                _parsedBankAccounts = importedData.BankAccounts ?? new List<BankAccount>();
                _parsedCategories = importedData.Categories ?? new List<Category>();
                _parsedOperations = importedData.Operations ?? new List<Operation>();

                Console.WriteLine($"Successfully imported {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON file: {ex.Message}");
            }
        }

        private class ImportedData
        {
            public List<BankAccount>? BankAccounts { get; set; } = new();
            public List<Category>? Categories { get; set; } = new();
            public List<Operation>? Operations { get; set; } = new();
        }
    }
}
