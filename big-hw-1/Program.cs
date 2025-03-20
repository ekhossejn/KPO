using Microsoft.Extensions.DependencyInjection;
using big_hw_1.facades;
using big_hw_1.menu;
using big_hw_1.storages;
namespace big_hw_1
{
    class Program
	{
        static void Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<BankAccountStorage>()
                .AddSingleton<CategoryStorage>()
                .AddSingleton<OperationStorage>()

                // proxy

                .AddSingleton<BankAccountFacade>()
                .AddSingleton<CategoryFacade>()
                .AddSingleton<OperationFacade>()
                .BuildServiceProvider();
            while (true)
            {
                try
                {
                    Menu menu = new(serviceProvider);
                    menu.Show();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
        }
    }
}

