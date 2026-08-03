

using MiniBank.Core;

public class BankServices : IBankServices {
    private readonly IAccountRepository _accountRepository ;
    private readonly ICustomerRepository _customerRepository ;

    public BankServices(IAccountRepository accountRepository, ICustomerRepository customerRepository){
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    public Account CreateAccount(int customerId, AccountType accountType, decimal initialDeposit = 0) {
        Customer customer = _customerRepository.GetByCustomerId(customerId)?? 
            throw new CustomerNotFoundException($"Customer with ID {customerId} was not found.");
        
        Account newAccount = accountType switch{
            AccountType.Current => new CurrentAccount(customer, initialDeposit),
            AccountType.Savings => new SavingsAccount(customer, initialDeposit),
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), $"Unsupported account type: {accountType}")
        };

        _accountRepository.Add(newAccount);

        return newAccount;
    }

    public Customer CreateCustomer(string fullName, string email, string phoneNumber) {
        var newCustomer = new Customer(fullName, email, phoneNumber);
        _customerRepository.Add(newCustomer);
        return newCustomer;
    }

    public void Deposit(int accountNumber, decimal amount) {
        var currentAccount = _accountRepository.GetByAccountNumber(accountNumber)??
            throw new AccountNotFoundException($"Account with Account Number {accountNumber} was not found.");

        currentAccount.Deposit(amount);

        var transaction = new Transaction(accountNumber, TransactionType.Deposit, amount, $"Account with Account Number {accountNumber} Deposited {amount}$");
        currentAccount.AddTransaction(transaction);

        _accountRepository.Update(currentAccount);
    }

    public Account? GetAccount(int accountNumber) {
        return _accountRepository.GetByAccountNumber(accountNumber);
    }

    public IEnumerable<Account> GetCustomerAccounts(int customerId) {
        var customer = _customerRepository.GetByCustomerId(customerId)?? 
            throw new CustomerNotFoundException($"Customer with ID {customerId} was not found.");

        return _accountRepository.GetByCustomerId(customerId);
    }

    public IEnumerable<Transaction> GetTransactionHistory(int accountNumber) {
        var currentAccount = _accountRepository.GetByAccountNumber(accountNumber)??
            throw new AccountNotFoundException($"Account with Account Number {accountNumber} was not found.");

        return currentAccount.GetTransactions();
    }

    public void Transfer(int fromAccount, int toAccount, decimal amount) {
        if (fromAccount == toAccount) throw new SameAccountTransferException();

        var currentFromAccount = _accountRepository.GetByAccountNumber(fromAccount)??
            throw new AccountNotFoundException($"Account with Account Number {fromAccount} was not found.");

        var currentToAccount = _accountRepository.GetByAccountNumber(toAccount)??
            throw new AccountNotFoundException($"Account with Account Number {toAccount} was not found.");

        currentFromAccount.Withdraw(amount);
        currentToAccount.Deposit(amount);

        var fromAccountTransaction = new Transaction(fromAccount, TransactionType.Transfer, amount, $"Account with Account Number {fromAccount} transfered {amount}$ to Account with Account Number {toAccount}");
        currentFromAccount.AddTransaction(fromAccountTransaction);

        var toAccountTransaction = new Transaction(fromAccount, TransactionType.Transfer, amount, $"Account with Account Number {toAccount} got an amount {amount}$ from Account with Account Number {fromAccount}");
        currentFromAccount.AddTransaction(toAccountTransaction);

        _accountRepository.Update(currentFromAccount);
        _accountRepository.Update(currentToAccount);
    }

    public void Withdraw(int accountNumber, decimal amount) {
        var currentAccount = _accountRepository.GetByAccountNumber(accountNumber)??
            throw new AccountNotFoundException($"Account with Account Number {accountNumber} was not found.");

        currentAccount.Withdraw(amount);

        var transaction = new Transaction(accountNumber, TransactionType.Withdrawal, amount, $"Account with Account Number {accountNumber} Withdraw {amount}$");
        currentAccount.AddTransaction(transaction);
        
        _accountRepository.Update(currentAccount);
    }
}