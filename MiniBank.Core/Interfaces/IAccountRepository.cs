using MiniBank.Core;

public interface IAccountRepository {
    void Add(Account account);
    Account? GetByAccountNumber(int accountNumber);

    IEnumerable<Account> GetByCustomerId(int customerId);
    IEnumerable<Account> GetAll();

    void Update(Account account);

}