using System;
using big_hw_1.facades;
using big_hw_1.models;
using big_hw_1.storages;
using big_hw_1.visitors;

namespace big_hw_1.commands
{
	public class ExportToFileCommand : ICommand
	{
        private readonly IFileExporterVisitor _visitor;
        private readonly BankAccountFacade _bankAccountFacade;
        private readonly OperationFacade _operationFacade;
        private readonly CategoryFacade _categoryFacade;
        private readonly string _filePath;

        public ExportToFileCommand(IFileExporterVisitor visitor, BankAccountFacade bankAccountFacade, OperationFacade operationFacade, CategoryFacade categoryFacade, string filePath) {
            _visitor = visitor;
            _bankAccountFacade = bankAccountFacade;
            _operationFacade = operationFacade;
            _categoryFacade = categoryFacade;
            _filePath = filePath;
        }

        public void Execute() {
            _visitor.Visit(_bankAccountFacade.Get());
            _visitor.Visit(_operationFacade.Get());
            _visitor.Visit(_categoryFacade.Get());
            _visitor.PushDataToFile(_filePath);
        }
    }
}
