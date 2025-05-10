using System;
using big_hw_1.models;
using big_hw_1.facades;

namespace big_hw_1.importers
{
	public abstract class FileImporter
	{
		protected readonly BankAccountFacade _bankAccountFacade;
		protected readonly CategoryFacade _categoryFacade;
		protected readonly OperationFacade _operationFacade;

		protected List<BankAccount> _parsedBankAccounts = new();
		protected List<Category> _parsedCategories = new();
		protected List<Operation> _parsedOperations = new();

		protected FileImporter(BankAccountFacade bankAccountFacade, CategoryFacade categoryFacade, OperationFacade operationFacade) {
			_bankAccountFacade = bankAccountFacade;
			_categoryFacade = categoryFacade;
			_operationFacade = operationFacade;
		}

		public void ImportFile(string filePath) {
			try {
				ReadAndParseFile(filePath);
				foreach (var bankAccount in _parsedBankAccounts) {
					_bankAccountFacade.Create(bankAccount.Id, bankAccount.Name, bankAccount.Balance);
				}
				foreach (var category in _parsedCategories) {
					_categoryFacade.Create(category.Id, category.Name, category.Type);
				}
				foreach (var operation in _parsedOperations) {
					_operationFacade.Create(operation.Id, operation.Type, operation.BankAccountId, operation.Amount, operation.Date, operation.Description, operation.CategoryId, false);
				}
			} catch (Exception e) {
				Console.WriteLine("Error while reading file: " + e.Message);
			}
		}

		protected abstract void ReadAndParseFile(string filePath);
	}
}
