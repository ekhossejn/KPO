using System;
using System.Xml.Linq;
using big_hw_1.factories;
using big_hw_1.models;
using big_hw_1.storages;

namespace big_hw_1.facades
{
	public class OperationFacade
	{
        private readonly IStorage<Operation> _operationStorage;
        private readonly IStorage<BankAccount> _bankAccountStorage;
        private readonly IStorage<Category> _categoryStorage;

        public OperationFacade(IStorage<Operation> operationStorage, IStorage<BankAccount> bankAccountStorage, IStorage<Category> categoryStorage)
        {
            _operationStorage = operationStorage;
            _bankAccountStorage = bankAccountStorage;
            _categoryStorage = categoryStorage;
        }

        public IEnumerable<Operation> Get() => _operationStorage.Get();

        public Operation? Get(Guid id) => _operationStorage.Get(id);

        public void Create(Guid id, models.Type type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId, bool update=true)
        {
            var bankAccount = _bankAccountStorage.Get(bankAccountId) ?? throw new ArgumentException("Unknown Bank Account id");
            var category = _categoryStorage.Get(categoryId) ?? throw new ArgumentException("Unknown Category id");
            if (type != category.Type)
            {
                throw new ArgumentException("Operation type doesn't equal to category type");
            }
            var operation = OperationFactory.Create(id, type, bankAccountId, amount, date, description, categoryId);
            _operationStorage.Add(operation);

            if (update)
            {
                bankAccount.ChangeBalance(type == models.Type.Income ? amount : -amount);
                _bankAccountStorage.Replace(bankAccount);
            }
        }

        public void Create(models.Type type, Guid bankAccountId, decimal amount, string description, Guid categoryId)
        {
            var bankAccount = _bankAccountStorage.Get(bankAccountId) ?? throw new ArgumentException("Unknown Bank Account id");
            var category = _categoryStorage.Get(categoryId) ?? throw new ArgumentException("Unknown Category id");
            if (type != category.Type)
            {
                throw new ArgumentException("Operation type doesn't equal to category type");
            }
            var operation = OperationFactory.CreateWithDefaultTime(type, bankAccountId, amount, description, categoryId);
            _operationStorage.Add(operation);
            bankAccount.ChangeBalance(type == models.Type.Income ? amount : -amount);
            _bankAccountStorage.Replace(bankAccount);
        }

        public void ChangeDescription(Guid id, string description)
        {
            var operation = _operationStorage.Get(id) ?? throw new ArgumentException("Unknown Operation id");
            operation.ChangeDescription(description);
            _operationStorage.Replace(operation);
        }

        public void ChangeCategoryId(Guid id, Guid categoryId)
        {
            var operation = _operationStorage.Get(id) ?? throw new ArgumentException("Unknown Operation id");
            var category = _categoryStorage.Get(categoryId) ?? throw new ArgumentException("Unknown Category id");
            if (operation.Type != category.Type)
            {
                throw new ArgumentException("Type of operation is not equal to type of new category");
            }
            operation.ChangeCategoryId(categoryId);
            _operationStorage.Replace(operation);
        }

        public void Delete(Guid id)
        {
            _operationStorage.Delete(id);
        }

    }
}

