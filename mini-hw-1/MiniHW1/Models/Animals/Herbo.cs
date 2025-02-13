namespace MiniHW1.Models
{
	public class Herbo : Animal, Interfaces.IKind
    {
        // Доброта других живых существ пока не имеет ограничений.
        private readonly int _minHerboKindLevel = 0;
        private readonly int _maxHerboKindLevel = 10;
        private readonly int _requiredHerboKindLevelForInteractiveZoo = 5;

        public int KindLevel { get; init; }

        public Herbo(int food, int health, int number, int kind): base(food, health, number)
        {
            if (kind < _minHerboKindLevel || kind > _maxHerboKindLevel)
            {
                throw new Exception("Доброта травоядного должна быть в пределах [0; 10]");
            }
            KindLevel = kind;
        }

        public bool IsAbleToBeInInteractiveZoo()
        {
            return KindLevel > _requiredHerboKindLevelForInteractiveZoo;
        }

        public override string ToString()
        {
            return $"{GetType().Name} имеет следующие параметры: food {Food}; health {Health}; number {Number}; kind_level {KindLevel} out of 10";
        }
    }
}

