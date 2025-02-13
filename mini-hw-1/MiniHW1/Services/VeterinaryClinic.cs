namespace MiniHW1.Services
{
	public class VeterinaryClinic : Interfaces.IHealthChecker
	{
		private readonly int _requiredHealth = 5;

		public bool IsHealthy(Interfaces.IAlive alive)
		{
			return alive.Health > _requiredHealth;
		}
	}
}

