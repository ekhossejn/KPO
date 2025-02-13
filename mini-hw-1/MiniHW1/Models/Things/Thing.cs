namespace MiniHW1.Models
{
	public abstract class Thing : Interfaces.IInventory
	{
		public int Number { get; set; }

        public Thing(int number)
        {
            Number = number;
        }

        public override string ToString()
        {
            return $"{GetType().Name} имеет следующие параметры: number {Number}";
        }
    }
}

