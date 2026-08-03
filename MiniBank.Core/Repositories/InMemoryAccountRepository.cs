
using MiniBank.Core;

public class InMemoryAccountRepository : IAccountRepository {
    private readonly Dictionary<int, Account> _accounts = new Dictionary<int, Account>();

    public void Add(Account account) {
        _accounts.Add(account.AccountNumber, account);
    }

    public IEnumerable<Account> GetAll() {
        return _accounts.Values;
    }

    public Account? GetByAccountNumber(int accountNumber) {
        return _accounts.GetValueOrDefault(accountNumber);
    }

    public IEnumerable<Account> GetByCustomerId(int customerId) {
        return _accounts.Values.Where(acc => acc.Customer.Id == customerId);
    }

    public void Update(Account account) {
        // Ensure the entity exists before overwriting
        if (!_accounts.ContainsKey(account.AccountNumber)) {
            throw new AccountNotFoundException($"Cannot update non-existent account #{account.AccountNumber}.");
        }

        _accounts[account.AccountNumber] = account;
    }

}