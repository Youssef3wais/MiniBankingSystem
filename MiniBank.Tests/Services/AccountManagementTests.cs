using MiniBank.Core;
using Moq;
using Xunit;

namespace MiniBank.Tests;

public class AccountManagementTests {
    private readonly Mock<IAccountRepository> _mockAccountRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly BankServices _bankServices;

    public AccountManagementTests() {
        _mockAccountRepo = new Mock<IAccountRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _bankServices = new BankServices(_mockAccountRepo.Object, _mockCustomerRepo.Object);
    }

    [Fact]
    public void CreateAccount_ValidCustomer_CreatesAccountAndAddsToRepository() {
        // Arrange
        var customer = new Customer("xxx", "xxx@email.com", "19929xxx");
        var customerId = customer.Id;

        _mockCustomerRepo
            .Setup(r => r.GetByCustomerId(customerId))
            .Returns(customer);

        // Act
        var account = _bankServices.CreateAccount(customerId, AccountType.Savings, 500);

        // Assert
        Assert.NotNull(account);
        Assert.IsType<SavingsAccount>(account);
        Assert.Equal(500, account.Balance);

        _mockAccountRepo.Verify(r => r.Add(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public void CreateAccount_CustomerNotFound_ThrowsCustomerNotFoundException() {
        // Arrange
        int invalidCustomerId = 999;
        _mockCustomerRepo
            .Setup(r => r.GetByCustomerId(invalidCustomerId))
            .Returns((Customer?)null);

        // Act & Assert
        Assert.Throws<CustomerNotFoundException>(() =>
            _bankServices.CreateAccount(invalidCustomerId, AccountType.Current, 100)
        );

        _mockAccountRepo.Verify(r => r.Add(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void CreateAccount_InvalidAccountType_ThrowsArgumentOutOfRangeException() {
        // Arrange
        var customer = new Customer("xxx", "xxx@email.com", "19929xxx");
        var customerId = customer.Id;

        _mockCustomerRepo
            .Setup(r => r.GetByCustomerId(customerId))
            .Returns(customer);

        // Cast an invalid integer to the enum to trigger the default switch case
        var invalidAccountType = (AccountType)99;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _bankServices.CreateAccount(customerId, invalidAccountType, 100)
        );
    }

    [Fact]
    public void GetCustomerAccounts_ValidCustomer_ReturnsListOfAccounts() {
        // Arrange
        var customer = new Customer("xxx", "xxx@email.com", "19929xxx");
        var customerId = customer.Id;

        var accounts = new List<Account>
        {
            new SavingsAccount(customer, 100),
            new CurrentAccount(customer, 200)
        };

        _mockCustomerRepo
            .Setup(r => r.GetByCustomerId(customerId))
            .Returns(customer);
        _mockAccountRepo
            .Setup(r => r.GetByCustomerId(customerId))
            .Returns(accounts);

        // Act
        var result = _bankServices.GetCustomerAccounts(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetCustomerAccounts_CustomerNotFound_ThrowsCustomerNotFoundException() {
        // Arrange
        int invalidCustomerId = 999;
        _mockCustomerRepo
            .Setup(r => r.GetByCustomerId(invalidCustomerId))
            .Returns((Customer?)null);

        // Act & Assert
        Assert.Throws<CustomerNotFoundException>(() =>
            _bankServices.GetCustomerAccounts(invalidCustomerId)
        );
    }

    [Fact]
    public void GetTransactionHistory_ValidAccount_ReturnsTransactions() {
        // Arrange
        var customer = new Customer("yyy", "yyy@email.com", "91991yy");
        var account = new SavingsAccount(customer, 500); // Initial deposit creates 1 transaction
        var accountNumber = account.AccountNumber;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(account.AccountNumber))
            .Returns(account);

        _bankServices.Deposit(accountNumber, 200);  //2nd transaction

        // Act
        var transactions = _bankServices.GetTransactionHistory(account.AccountNumber);

        // Assert
        Assert.NotNull(transactions);
        Assert.Equal(2, transactions.Count());
    }

    [Fact]
    public void GetTransactionHistory_AccountNotFound_ThrowsAccountNotFoundException() {
        // Arrange
        int invalidAccountNumber = 9999;
        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(invalidAccountNumber))
            .Returns((Account?)null);

        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() =>
            _bankServices.GetTransactionHistory(invalidAccountNumber)
        );
    }


}