namespace MiniHW1.Models
{
	public abstract class Animal: Interfaces.IAlive, Interfaces.IInventory
	{
		public int Food { get; set; }
		public int Health { get; set; }
		public int Number { get; set; }

        public Animal(int food, int health, int number)
        {
            Food = food;
            Health = health;
            Number = number;
        }

        public override string ToString()
        {
            return $"{GetType().Name} имеет следующие параметры food: {Food}; health {Health}; number {Number}";
        }
    }
}
