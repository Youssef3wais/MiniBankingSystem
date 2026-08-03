using System.Linq;
using Xunit;

namespace MiniBank.Tests;

public class RepositoryTests {
    #region Customer Repository Tests

    [Fact]
    public void AddCustomer_AssignsAutoIncrementedIdAndStoresCustomer() {
        // Arrange
        var customerRepo = new InMemoryCustomerRepository();
        var customer1 = new Customer("xxx", "xxx@email.com", "123xxx");
        var customer2 = new Customer("yyy", "yyy@email.com", "123yyy");

        // Act
        customerRepo.Add(customer1);
        customerRepo.Add(customer2);

        // Assert
        Assert.Equal(101, customer1.Id);
        Assert.Equal(102, customer2.Id);

        var retrieved = customerRepo.GetByCustomerId(101);
        Assert.NotNull(retrieved);
        Assert.Equal("xxx", retrieved.FullName);
    }

    [Fact]
    public void GetByCustomerId_NonExistentId_ReturnsNull() {
        // Arrange
        var customerRepo = new InMemoryCustomerRepository();

        // Act
        var result = customerRepo.GetByCustomerId(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAllCustomers_ReturnsAllStoredCustomers() {
        // Arrange
        var customerRepo = new InMemoryCustomerRepository();
        customerRepo.Add(new Customer("xxx", "xxx@email.com", "123xxx"));
        customerRepo.Add(new Customer("yyy", "yyy@email.com", "123yyy"));

        // Act
        var customers = customerRepo.GetAll();

        // Assert
        Assert.Equal(2, customers.Count());
    }

    #endregion

    #region Account Repository Tests

    [Fact]
    public void AddAccount_StoresAccountSuccessfully() {
        // Arrange
        var accountRepo = new InMemoryAccountRepository();
        var customer = new Customer("xxx", "xx@email.com", "123xxx") { Id = 1 };
        var account = new SavingsAccount(customer, 500);

        // Act
        accountRepo.Add(account);

        // Assert
        var retrieved = accountRepo.GetByAccountNumber(account.AccountNumber);
        Assert.NotNull(retrieved);
        Assert.Equal(500, retrieved.Balance);
    }

    [Fact]
    public void GetByCustomerId_ReturnsOnlyAccountsBelongingToCustomer() {
        // Arrange
        var accountRepo = new InMemoryAccountRepository();
        var customer1 = new Customer("User One", "one@email.com", "111") { Id = 1 };
        var customer2 = new Customer("User Two", "two@email.com", "222") { Id = 2 };

        var acc1 = new SavingsAccount(customer1, 100);
        var acc2 = new CurrentAccount(customer1, 200);
        var acc3 = new SavingsAccount(customer2, 300);

        accountRepo.Add(acc1);
        accountRepo.Add(acc2);
        accountRepo.Add(acc3);

        // Act
        var customer1Accounts = accountRepo.GetByCustomerId(1);

        // Assert
        Assert.Equal(2, customer1Accounts.Count());
        Assert.All(customer1Accounts, a => Assert.Equal(1, a.Customer.Id));
    }

    [Fact]
    public void UpdateAccount_ExistingAccount_UpdatesStateSuccessfully() {
        // Arrange
        var accountRepo = new InMemoryAccountRepository();
        var customer = new Customer("Alice", "alice@email.com", "123") { Id = 1 };
        var account = new SavingsAccount(customer, 100);
        accountRepo.Add(account);

        // Act: Mutate state and update
        account.Deposit(150);
        accountRepo.Update(account);

        // Assert
        var updatedAccount = accountRepo.GetByAccountNumber(account.AccountNumber);
        Assert.NotNull(updatedAccount);
        Assert.Equal(250, updatedAccount.Balance);
    }

    [Fact]
    public void UpdateAccount_NonExistentAccount_ThrowsAccountNotFoundException() {
        // Arrange
        var accountRepo = new InMemoryAccountRepository();
        var customer = new Customer("Ghost", "ghost@email.com", "000") { Id = 1 };
        var unbackedAccount = new SavingsAccount(customer, 100);

        // Act & Assert (Updating an account that was never added to the repo should throw)
        Assert.Throws<AccountNotFoundException>(() =>
            accountRepo.Update(unbackedAccount)
        );
    }

    #endregion
}