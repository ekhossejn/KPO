using System;
namespace Zoo.Domain.ValueObjects
{
	public record DateField
	{
        public DateOnly Date { get; }

        public DateField(string value)
		{
            bool success = DateOnly.TryParse(value, out DateOnly date);
            if (!success)
            {
                throw new ArgumentException("Incorrect date. Must be in the format yyyy-MM-dd.");
            }
            Date = date;
        }
	}
}

