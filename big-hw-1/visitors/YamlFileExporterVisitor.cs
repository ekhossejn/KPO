using System;
using big_hw_1.models;

namespace big_hw_1.visitors
{
	public class YamlFileExporterVisitor : IFileExporterVisitor
	{
		public void Visit(IEnumerable<BankAccount> bankAccounts) {}

		public void Visit(IEnumerable<Category> categories) {}

		public void Visit(IEnumerable<Operation> operations) {}

		public void PushDataToFile(string filePath) {
			Console.WriteLine($"Writing data to {filePath}");
		}
	}
}
