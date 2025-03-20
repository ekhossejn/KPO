using System;
namespace big_hw_1.models
{
	public class Operation
	{
		public Guid Id { get; private set; }
		public Type Type { get; private set; }
		public Guid BankAccountId { get; private set; }
		private decimal _amount;
		public decimal Amount
		{
			get => _amount;
			private set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("Amount must be positive");
                }
				_amount = value;
			}
		}
		public DateTime Date { get; private set; }
        public string Description { get; private set; }
        public Guid CategoryId { get; private set; }

        public Operation(Guid id, Type type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId)
		{
			Id = id;
			Type = type;
			BankAccountId = bankAccountId;
			Amount = amount;
			Date = date;
			Description = description;
			CategoryId = categoryId;
		}

        public void ChangeDescription(string newDescription)
        {
            Description = newDescription;
        }

        public void ChangeCategoryId(Guid newCategoryId)
        {
            CategoryId = newCategoryId;
        }

    }
}

