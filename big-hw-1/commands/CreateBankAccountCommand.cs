using System;
using big_hw_1.facades;

namespace big_hw_1.commands
{
	public class CreateBankAccountCommand : ICommand
	{
        private readonly BankAccountFacade _bankAccountFacade;
        private readonly string _accountName;
        private readonly decimal _accountBalance;
        
        public CreateBankAccountCommand(BankAccountFacade bankAccountFacade, string accountName, decimal accountBalance) {
            _bankAccountFacade = bankAccountFacade;
            _accountName = accountName;
            _accountBalance = accountBalance;
        }

        public void Execute() {
            _bankAccountFacade.Create(_accountName, _accountBalance);
        }
    }
}

