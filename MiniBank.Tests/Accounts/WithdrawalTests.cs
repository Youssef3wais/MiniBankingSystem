using MiniBank.Core;
using Moq;
using Xunit;

namespace MiniBank.Tests;

public class WithdrawalTests {
    private readonly Mock<IAccountRepository> _mockAccountRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly BankServices _bankServices;

    public WithdrawalTests() {
        _mockAccountRepo = new Mock<IAccountRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _bankServices = new BankServices(_mockAccountRepo.Object, _mockCustomerRepo.Object);
    }

    [Fact]
    public void Withdraw_ValidAmount_DecreasesBalanceAndUpdatesRepository() {
        //Arrange
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new CurrentAccount(customer, 1000);
        int accountNumber = account.AccountNumber ;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);

        //Act
        _bankServices.Withdraw(accountNumber, 500);

        //Assert
        Assert.Equal(500, account.Balance);
        _mockAccountRepo.Verify(r => r.Update(account), Times.Once);
    }

    [Fact]
    public void Withdraw_ValidAmount_CreatesTransactionRecord() {
        //Arrange
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new CurrentAccount(customer, 1000);
        int accountNumber = account.AccountNumber ;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);
        
        //act
        _bankServices.Withdraw(accountNumber, 200);

        //Assert
        Assert.Equal(800, account.Balance);
        var transactions = _bankServices.GetTransactionHistory(accountNumber);
        Assert.Equal(2, transactions.Count());
    }
    [Fact]
    public void Withdraw_SavingsAccount_ExceedingBalance_ThrowsInsufficientFundsException() {
        //Arrange
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new SavingsAccount(customer, 100);
        int accountNumber = account.AccountNumber ;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);
        

        //Act & Assert
        Assert.Throws<InsufficientFundsException>(()=>_bankServices.Withdraw(accountNumber, 200));

        // Verify state remains untouched and Update was never called
        Assert.Equal(100, account.Balance);
        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Withdraw_CurrentAccount_WithinOverdraftLimit_AllowsWithdrawal()
    {
        //Arrange
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new CurrentAccount(customer, 100);
        int accountNumber = account.AccountNumber ;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);

        // Act 
        _bankServices.Withdraw(accountNumber, 300);

        // Assert
        Assert.Equal(-200, account.Balance);
        _mockAccountRepo.Verify(r => r.Update(account), Times.Once);
    }

    [Fact]
    public void Withdraw_CurrentAccount_ExceedingOverdraftLimit_ThrowsInsufficientFundsException() {
        //Arrange
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new CurrentAccount(customer, 100);
        int accountNumber = account.AccountNumber ;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);

        //Act & Assert
        Assert.Throws<InsufficientFundsException>(()=>_bankServices.Withdraw(accountNumber, 900));

        // Verify state remains untouched and Update was never called
        Assert.Equal(100, account.Balance);
        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    [InlineData(-0.01)]
    public void Withdraw_ZeroOrNegativeAmount_ThrowsInvalidAmountException(decimal invalidAmount) {
        //Arrange
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new CurrentAccount(customer, 100);
        int accountNumber = account.AccountNumber ;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);

        //Act & Assert
        Assert.Throws<InvalidAmountException>(()=>_bankServices.Withdraw(accountNumber, invalidAmount));

        // Verify state remains untouched and Update was never called
        Assert.Equal(100, account.Balance);
        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Withdraw_NonExistentAccount_ThrowsAccountNotFoundException() {
        // Arrange
        int invalidAccountNumber = 7;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(invalidAccountNumber))
            .Returns((Account?)null);

        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() =>
            _bankServices.Withdraw(invalidAccountNumber, 50)
        );

        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

}