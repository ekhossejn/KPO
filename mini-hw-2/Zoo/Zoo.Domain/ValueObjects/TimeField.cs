using System;
namespace Zoo.Domain.ValueObjects
{
	public record TimeField
	{
        public TimeOnly Time { get; }

        public TimeField(string value)
        {
            bool success = TimeOnly.TryParse(value, out TimeOnly time);
            if (!success)
            {
                throw new ArgumentException("Incorrect time.");
            }
            Time = time;
        }
    }
}

