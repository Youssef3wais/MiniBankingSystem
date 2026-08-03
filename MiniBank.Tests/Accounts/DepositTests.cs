using MiniBank.Core;
using Moq;
using Xunit;

namespace MiniBank.Tests;

public class DepositTests {
    private readonly Mock<IAccountRepository> _mockAccountRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly BankServices _bankServices;

    public DepositTests() {
        
        _mockAccountRepo = new Mock<IAccountRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();

        _bankServices = new BankServices(_mockAccountRepo.Object, _mockCustomerRepo.Object);
    }

    [Fact]
    public void Deposit_ValidAmount_IncreasesAccountBalanceAndUpdatesRepository() {
        //Arrange
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new SavingsAccount(customer, balance: 100);
        int accountNumber = account.AccountNumber;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);


        //Act
        _bankServices.Deposit(accountNumber, 50);

        // Assert
        Assert.Equal(150, account.Balance);

        // Moq Feature: Verify that Update() was explicitly called once with the updated account
        _mockAccountRepo.Verify(r => r.Update(account), Times.Once);
    }

    [Fact]
    public void Deposit_ValidAmount_CreatesTransactionRecord() {
        //Arrange 
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new SavingsAccount(customer, balance: 1000);
        int accountNumber = account.AccountNumber;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);
        
        //Act
        _bankServices.Deposit(accountNumber, 500);
        
        //Assert
        var Transaction = _bankServices.GetTransactionHistory(accountNumber);
        Assert.Equal(2, Transaction.Count());
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    [InlineData(-0.01)]
    public void Deposit_ZeroOrNegativeAmount_ThrowsInvalidAmountException(decimal invalidAmount) {
        //Arrange 
        var customer = new Customer("Yusef", "youssefewais@email.com", "01158110");
        var account = new SavingsAccount(customer, balance: 1000);
        int accountNumber = account.AccountNumber;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(accountNumber))
            .Returns(account);
        
        //Act & Assert
        Assert.Throws<InvalidAmountException>(() => 
            _bankServices.Deposit(accountNumber, invalidAmount)
        );

        // Moq Feature: Verify Update() was NEVER called because the deposit failed
        _mockAccountRepo.Verify(r => r.Update(account), Times.Never);
    }

    [Fact]
    public void Deposit_NonExistentAccount_ThrowsAccountNotFoundException() {
        //Arrange 
        int invalidAccountNumber = 9999;

        _mockAccountRepo
            .Setup(r => r.GetByAccountNumber(invalidAccountNumber))
            .Returns((Account?)null);


        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() => 
            _bankServices.Deposit(invalidAccountNumber, 100)
        );

        // Moq Feature: Verify Update() was NEVER called
        _mockAccountRepo.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }






}