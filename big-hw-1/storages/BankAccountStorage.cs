using System;
using big_hw_1.models;
namespace big_hw_1.storages
{
	public class BankAccountStorage
	{
		private readonly Dictionary<Guid, BankAccount> _bankAccountStorage = new();

		public void Add(BankAccount bankAccount)
		{
			if (_bankAccountStorage.ContainsKey(bankAccount.Id))
			{
				throw new ArgumentException("Duplicated Bank Account id");
			}
			_bankAccountStorage[bankAccount.Id] = bankAccount;
		}

        public BankAccount? Get(Guid id) => _bankAccountStorage.TryGetValue(id, out var res) ? res : null;

        public IEnumerable<BankAccount> Get() => _bankAccountStorage.Values;

		public void Delete(Guid id)
		{
            _bankAccountStorage.Remove(id);
		}

		public void Replace(BankAccount bankAccount)
		{
            if (!_bankAccountStorage.ContainsKey(bankAccount.Id))
            {
                throw new ArgumentException("Unknown Bank Account id");
            }
            _bankAccountStorage[bankAccount.Id] = bankAccount;
        }
	}
}

