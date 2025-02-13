namespace MiniHW1.Interfaces
{
	public interface IHealthChecker
	{
        public bool IsHealthy(IAlive alive);
    }
}
