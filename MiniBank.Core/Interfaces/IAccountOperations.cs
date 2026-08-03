

public interface IAccountOperations {
    int AccountNumber{get; }
    decimal Balance{get; }

    void Deposit(decimal amount);
    void Withdraw(decimal amount);

    IReadOnlyCollection<Transaction> GetTransactions();

}