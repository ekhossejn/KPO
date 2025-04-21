using System;
namespace Zoo.Domain.ValueObjects
{
	public record PositiveInt
	{
		public int Value { get; }

		public PositiveInt(int value)
		{
			if (value <= 0)
			{
                throw new ArgumentException("Value must be bigger than 0.");
            }
			Value = value;
		}
	}
}

