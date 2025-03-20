using System;
namespace big_hw_1.models
{
	public class BankAccount
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Balance must not be negative");
                }
                _balance = value;
            }
        }

        public BankAccount(Guid id, string name, decimal balance)
        {
            Id = id;
            Name = name;
            Balance = balance;
        }

        public void ChangeBalance(decimal deltaBalance)
        {
            Balance += deltaBalance;
        }

        public void ChangeName(string newName)
        {
            Name = newName;
        }
    }
}

