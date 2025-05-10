using System;
using big_hw_1.models;

namespace big_hw_1.visitors
{
	public interface IFileExporterVisitor
	{
		void Visit(IEnumerable<BankAccount> bankAccounts);

		void Visit(IEnumerable<Category> categories);

		void Visit(IEnumerable<Operation> operations);

		void PushDataToFile(string filePath);
	}
}
