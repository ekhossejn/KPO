using System;
using big_hw_1.models;
namespace big_hw_1.factories
{
	public class OperationFactory
	{
        public static Operation Create(Guid id, models.Type type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Amount must be positive");
            }
            return new Operation(id, type, bankAccountId, amount, date, description, categoryId);
        }

        public static Operation Create(models.Type type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Amount must be positive");
            }
            return new Operation(Guid.NewGuid(), type, bankAccountId, amount, date, description, categoryId);
        }

        public static Operation CreateWithDefaultTime(models.Type type, Guid bankAccountId, decimal amount, string description, Guid categoryId)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Amount must be positive");
            }
            var date = DateTime.Now;
            return new Operation(Guid.NewGuid(), type, bankAccountId, amount, date, description, categoryId);
        }
    }
}

