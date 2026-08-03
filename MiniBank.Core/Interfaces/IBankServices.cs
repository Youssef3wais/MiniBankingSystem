
using MiniBank.Core;

public interface IBankServices {
    Customer CreateCustomer(string fullName, string email, string phoneNumber);
    Account CreateAccount(int customerId, AccountType accountType, decimal initialDeposit = 0);

    void Deposit(int accountNumber, decimal amount);
    void Withdraw(int accountNumber, decimal amount);

    void Transfer(int fromAccount, int toAccount, decimal amount);

    Account? GetAccount(int accountNumber);

    IEnumerable<Account> GetCustomerAccounts(int customerId);

    IEnumerable<Transaction> GetTransactionHistory(int accountNumber);


}