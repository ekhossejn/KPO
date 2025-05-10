using System;
using big_hw_1.storages;
using big_hw_1.factories;
using big_hw_1.models;
namespace big_hw_1.facades
{
	public class BankAccountFacade
	{
		private readonly IStorage<BankAccount> _bankAccountStorage;

        public BankAccountFacade(IStorage<BankAccount> bankAccountStorage)
		{
			_bankAccountStorage = bankAccountStorage;
        }

		public IEnumerable<BankAccount> Get() => _bankAccountStorage.Get();

        public BankAccount? Get(Guid id) => _bankAccountStorage.Get(id);

        public void Create(Guid id, string name, decimal balance)
        {
            var bankAccount = BankAccountFactory.Create(id, name, balance);
            _bankAccountStorage.Add(bankAccount);
        }

        public void Create(string name, decimal balance)
        {
            var bankAccount = BankAccountFactory.Create(name, balance);
            _bankAccountStorage.Add(bankAccount);
        }

        public void ChangeBalance(Guid id, decimal deltaBalance)
        {
            var bankAccount = _bankAccountStorage.Get(id) ?? throw new ArgumentException("Unknown Bank Account id");
            bankAccount.ChangeBalance(deltaBalance);
            _bankAccountStorage.Replace(bankAccount);
        }

        public void ChangeName(Guid id, string name)
        {
            var bankAccount = _bankAccountStorage.Get(id) ?? throw new ArgumentException("Unknown Bank Account id");
            bankAccount.ChangeName(name);
            _bankAccountStorage.Replace(bankAccount);
        }

        public void Delete(Guid id)
        {
            _bankAccountStorage.Delete(id);
        }
    }
}

