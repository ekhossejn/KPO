using System;
using big_hw_1.facades;
namespace big_hw_1.commands
{
	public class CreateOperationCommand : ICommand
	{
        private readonly OperationFacade _operationFacade;
        readonly models.Type _type;
        readonly Guid _bankAccountId;
        readonly decimal _amount;
        readonly string _description;
        readonly Guid _categoryId;

        public CreateOperationCommand(OperationFacade operationFacade, models.Type type, Guid bankAccountId, decimal amount, string description, Guid categoryId) {
            _operationFacade = operationFacade;
            _type = type;
            _bankAccountId = bankAccountId;
            _amount = amount;
            _description = description;
            _categoryId = categoryId;
        }

        public void Execute() {
            _operationFacade.Create(_type, _bankAccountId, _amount, _description, _categoryId);
        }
    }
}

