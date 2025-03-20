using System;
using big_hw_1.models;
namespace big_hw_1.factories
{
	public class BankAccountFactory
	{
		public static BankAccount Create(string name, decimal balance)
		{
			if (balance < 0)
			{
				throw new ArgumentOutOfRangeException("Balance must not be negative");
			}
			return new BankAccount(Guid.NewGuid(), name, balance);
		}

        public static BankAccount CreateWithDefaultName(decimal balance)
        {
            return new BankAccount(Guid.NewGuid(), "default_name_" + Guid.NewGuid().ToString(), balance);
        }
    }
}

