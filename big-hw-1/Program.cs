using Microsoft.Extensions.DependencyInjection;
using big_hw_1.facades;
using big_hw_1.menu;
using big_hw_1.storages;
using big_hw_1.models;

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
                .AddSingleton<IStorage<BankAccount>>(provider =>
                    new StorageProxy<BankAccount>(provider.GetRequiredService<BankAccountStorage>()))
                .AddSingleton<IStorage<Category>>(provider =>
                    new StorageProxy<Category>(provider.GetRequiredService<CategoryStorage>()))
                .AddSingleton<IStorage<Operation>>(provider =>
                    new StorageProxy<Operation>(provider.GetRequiredService<OperationStorage>()))
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

